using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Recipes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class UploadController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("/api/Upload")]
    public async Task<IActionResult> UploadImage()
    {
        if (!Request.HasFormContentType)
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
        }

        try
        {
            var root = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(root))
            {
                root = _environment.ContentRootPath;
            }

            if (string.IsNullOrWhiteSpace(root))
            {
                root = Directory.GetCurrentDirectory();
            }

            var tempDir = Path.Combine(root, "temp");
            Directory.CreateDirectory(tempDir);

            var saved = new List<string>();
            foreach (IFormFile file in Request.Form.Files)
            {
                if (file?.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName ?? string.Empty);
                    var fullPath = Path.Combine(tempDir, fileName);
                    await using var stream = System.IO.File.Create(fullPath);
                    await file.CopyToAsync(stream);
                    saved.Add(fullPath);
                }
            }

            return Ok(new { count = saved.Count, files = saved });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadImage failed");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/attachment/{codeliste:int}")]
    public ActionResult<List<RecipeAttachment>> GetAttachment(int codeliste)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT ID, Flag, Filename, Filecaption, [Default] from EgswListeFiles where codeliste=@CodeListe ORDER BY [ID]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<RecipeAttachment>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new RecipeAttachment
                {
                    Id = GetInt(r["ID"]),
                    Name = GetStr(r["Filecaption"]),
                    Resource = GetStr(r["Filename"]),
                    Type = GetInt(r["Flag"]),
                    IsDefault = GetInt(r["Default"]) == 1
                });
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Missing or invalid parameters", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAttachment failed for {CodeListe}", codeliste);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/picture/{codeliste:int}")]
    public ActionResult<List<GenericList>> GetPicture(int codeliste)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT Picturename from EgswListe where code=@CodeListe";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var picture = string.Empty;
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var name = GetStr(r["Picturename"]);
                if (!string.IsNullOrEmpty(name) && name.EndsWith(";", StringComparison.Ordinal))
                {
                    name = name[..^1];
                }

                picture = name;
            }

            var pics = new List<GenericList>();
            var ctr = 1;
            foreach (var pic in (picture ?? string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                pics.Add(new GenericList { Code = ctr, Value = GetStr(pic) });
                ctr++;
            }

            return Ok(pics);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Missing or invalid parameters", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPicture failed for {CodeListe}", codeliste);
            return StatusCode(500);
        }
    }

    private static int GetInt(object value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
        try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
    }

    private static string GetStr(object value, string fallback = "")
    {
        if (value == null || value == DBNull.Value) return fallback;
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }
}
