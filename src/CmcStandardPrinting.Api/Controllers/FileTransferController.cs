using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using CmcStandardPrinting.Domain.Uploads;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Imaging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class FileTransferController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileTransferController> _logger;

    public FileTransferController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<FileTransferController> logger)
    {
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    private bool DebugEnabled => _configuration.GetValue("DebugEnabled", false);

    private string TempFolder
    {
        get
        {
            var configured = _configuration["TempFolder"];
            var root = string.IsNullOrWhiteSpace(configured)
                ? (_environment.WebRootPath ?? _environment.ContentRootPath ?? Directory.GetCurrentDirectory())
                : configured;

            var path = Path.Combine(root, "temp");
            Directory.CreateDirectory(path);
            return path;
        }
    }

    [HttpHead("/FileTransferHandler.ashx")]
    [HttpGet("/FileTransferHandler.ashx")]
    public IActionResult Get([FromQuery(Name = "f")] string? file)
    {
        Response.Headers.Add("Pragma", "no-cache");
        Response.Headers.Add("Cache-Control", "private, no-cache");

        if (string.IsNullOrWhiteSpace(file))
        {
            // Legacy handler silently ignored list requests; return empty payload for compatibility.
            return Ok(Array.Empty<FileTransferStatus>());
        }

        return DeliverFile(file);
    }

    [HttpPost("/FileTransferHandler.ashx")]
    [HttpPut("/FileTransferHandler.ashx")]
    public async Task<IActionResult> Upload()
    {
        Response.Headers.Add("Pragma", "no-cache");
        Response.Headers.Add("Cache-Control", "private, no-cache");

        var statuses = new List<FileTransferStatus>();
        try
        {
            var headers = Request.Headers;
            if (!string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                await UploadPartialFile(headers["X-File-Name"]!, statuses);
            }
            else
            {
                await UploadWholeFile(statuses);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File upload failed");
            statuses.Add(new FileTransferStatus { ResultCode = 440, Error = "Upload failed" });
        }

        SetJsonContentType();
        return Ok(statuses.ToArray());
    }

    [HttpDelete("/FileTransferHandler.ashx")]
    public IActionResult Delete([FromQuery(Name = "f")] string? file)
    {
        Response.Headers.Add("Pragma", "no-cache");
        Response.Headers.Add("Cache-Control", "private, no-cache");

        if (string.IsNullOrWhiteSpace(file))
        {
            return BadRequest();
        }

        try
        {
            var filePath = Path.Combine(TempFolder, file);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to delete file {File}", file);
        }

        return Ok();
    }

    [HttpOptions("/FileTransferHandler.ashx")]
    public IActionResult Options()
    {
        Response.Headers.Add("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
        return Ok();
    }

    private async Task UploadPartialFile(string fileName, List<FileTransferStatus> statuses)
    {
        var hasForm = Request.HasFormContentType;
        var files = hasForm ? Request.Form.Files : null;
        var fileCount = files?.Count ?? 0;

        if (fileCount > 1)
        {
            throw new InvalidOperationException("Attempt to upload chunked file containing more than one fragment per request");
        }

        var fullName = Path.Combine(TempFolder, Path.GetFileName(fileName));
        if (fileCount == 1 && files is not null)
        {
            var file = files[0];
            await using var target = System.IO.File.Open(fullName, FileMode.Append, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(target);
        }
        else
        {
            await using var target = System.IO.File.Open(fullName, FileMode.Append, FileAccess.Write, FileShare.None);
            await Request.Body.CopyToAsync(target);
        }

        statuses.Add(FileTransferStatus.FromFile(new FileInfo(fullName)));
    }

    private async Task UploadWholeFile(List<FileTransferStatus> statuses)
    {
        if (Request.Form?.Files?.Count > 0)
        {
            foreach (var file in Request.Form.Files)
            {
                if (file is null)
                {
                    continue;
                }

                var generatedName = BuildFileName(file);
                if (string.IsNullOrEmpty(generatedName))
                {
                    statuses.Add(new FileTransferStatus { ResultCode = 440, Error = "Invalid file type" });
                    continue;
                }

                var fullPath = Path.Combine(TempFolder, generatedName);
                var contentGroup = GetContentGroup(file.ContentType);

                try
                {
                    await using var stream = System.IO.File.Create(fullPath);
                    await file.CopyToAsync(stream);
                    if (string.Equals(contentGroup, "image", StringComparison.OrdinalIgnoreCase))
                    {
                        TestRotate(fullPath);
                    }

                    statuses.Add(FileTransferStatus.FromFile(generatedName, (int)file.Length));
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid file type for {File}", generatedName);
                    statuses.Add(new FileTransferStatus { ResultCode = 440, Error = "Invalid file type" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to upload file {File}", generatedName);
                    statuses.Add(new FileTransferStatus { ResultCode = 440, Error = "Upload failed" });
                }
            }
        }
    }

    private IActionResult DeliverFile(string file)
    {
        try
        {
            var filePath = Path.Combine(TempFolder, file);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = "application/octet-stream";
            return PhysicalFile(filePath, contentType, file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to deliver file {File}", file);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private void SetJsonContentType()
    {
        var accept = Request.Headers["HTTP_ACCEPT"].FirstOrDefault();
        if (!string.IsNullOrEmpty(accept) && accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            Response.ContentType = "application/json";
        }
        else
        {
            Response.ContentType = "text/plain";
        }
    }

    private void TestRotate(string filePath)
    {
        try
        {
            using var img = Image.FromFile(filePath);
            if (!img.PropertyIdList.Contains(0x0112))
            {
                return;
            }

            var propOrientation = img.GetPropertyItem(0x0112);
            var orientation = BitConverter.ToInt16(propOrientation.Value, 0);

            if (orientation == 6)
            {
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            else if (orientation == 8)
            {
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }

            img.RemovePropertyItem(0x0112);
            img.Save(filePath, ImageFormat.Jpeg);
        }
        catch (Exception ex)
        {
            if (DebugEnabled)
            {
                _logger.LogInformation(ex, "Image rotation skipped for {File}", filePath);
            }
        }
    }

    private string? BuildFileName(IFormFile file)
    {
        var fieldName = file.Name ?? string.Empty;
        var extension = Path.GetExtension(file.FileName ?? string.Empty);
        var contentGroup = GetContentGroup(file.ContentType);
        var timestamp = DateTime.UtcNow.ToString("MMddyyHHmmss", CultureInfo.InvariantCulture);

        if (fieldName.Equals("filepicture", StringComparison.OrdinalIgnoreCase) && Request.Form.Count > 0)
        {
            if (!string.Equals(contentGroup, "image", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var pictureNumber = GetInt(Request.Form.First().Value, 1);
            return $"P{timestamp}_{pictureNumber}{extension}";
        }

        if (fieldName.Contains("fileprocpicture", StringComparison.OrdinalIgnoreCase) || fieldName.Contains("filecookbookpicture", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(contentGroup, "image", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var suffix = fieldName.Length > 15 ? fieldName.Substring(15) : string.Empty;
            var pictureNumber = GetInt(suffix);
            return $"P{timestamp}_{pictureNumber}{extension}";
        }

        if (fieldName.Contains("fileToUpload", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(contentGroup, "image", StringComparison.OrdinalIgnoreCase))
            {
                return $"P{timestamp}{extension}";
            }

            if (string.Equals(contentGroup, "video", StringComparison.OrdinalIgnoreCase))
            {
                return $"V{timestamp}{extension}";
            }

            return null;
        }

        return Path.GetFileName(file.FileName ?? string.Empty);
    }

    private static string GetContentGroup(string? contentType)
    {
        return (contentType ?? string.Empty).Split('/').FirstOrDefault() ?? string.Empty;
    }

    private static int GetInt(string? value, int defaultValue = 0)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : defaultValue;
    }
}
