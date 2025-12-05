using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class UploadController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpPost("/api/Upload")]
        public async Task<IActionResult> UploadImage()
        {
            if (!Request.HasFormContentType)
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType);
            }

            var env = HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
            var root = env?.WebRootPath ?? env?.ContentRootPath ?? Directory.GetCurrentDirectory();
            var tempDir = Path.Combine(root, "temp");
            Directory.CreateDirectory(tempDir);

            var saved = new List<string>();
            foreach (IFormFile file in Request.Form.Files)
            {
                if (file?.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName ?? string.Empty);
                    var fullPath = Path.Combine(tempDir, fileName);
                    using var stream = System.IO.File.Create(fullPath);
                    await file.CopyToAsync(stream);
                    saved.Add(fullPath);
                }
            }

            return Ok(new { count = saved.Count, files = saved });
        }

        [HttpGet("/api/attachment/{codeliste:int}")]
        public ActionResult<List<Models.RecipeAttachment>> GetAttachment(int codeliste)
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

                var list = new List<Models.RecipeAttachment>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.RecipeAttachment
                    {
                        Id = GetInt(r["ID"]),
                        Name = GetStr(r["Filecaption"]),
                        Resource = GetStr(r["Filename"]),
                        Type = GetInt(r["Flag"]),
                        IsDefault = GetInt(r["Default"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("/api/picture/{codeliste:int}")]
        public ActionResult<List<Models.GenericList>> GetPicture(int codeliste)
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
                        name = name.Substring(0, name.Length - 1);
                    }
                    picture = name;
                }

                var pics = new List<Models.GenericList>();
                var ctr = 1;
                foreach (var pic in (picture ?? string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    pics.Add(new Models.GenericList { Code = ctr, Value = GetStr(pic) });
                    ctr++;
                }
                return Ok(pics);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // Helpers
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
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
