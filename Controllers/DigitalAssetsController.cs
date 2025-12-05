using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DigitalAssetsController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("imagetype")]
        public ActionResult<List<Models.GenericCodeValueList>> GetImageType()
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM EgswDigitalAssetsMediaTypes WHERE [Name] IN('JPEG','PNG')";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var imagetype = new List<Models.GenericCodeValueList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    imagetype.Add(new Models.GenericCodeValueList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
                return Ok(imagetype);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("{codesite:int}/{codetrans:int}/{codeuser:int}/{codemediatype:int}/{bitKeywordAnd:bool}/{name?}")]
        public ActionResult<Models.ResponseDigitalAssets> GetDigitalAssets(int codesite, int codetrans, int codeuser, int codemediatype, bool bitKeywordAnd, string? name = "", string? keyword = "", string? recipenumber = "", int recipenumberAnd = 0, string? recipename = "", int take = 10, int skip = 0)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(name)) name = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(name));
                if (!string.IsNullOrWhiteSpace(keyword)) keyword = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(keyword));
                if (!string.IsNullOrWhiteSpace(recipename)) recipename = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(recipename));
                if (!string.IsNullOrWhiteSpace(recipenumber)) recipenumber = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(recipenumber));

                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[sp_EgswDigitalAssetsearch3]";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 4000).Value = name ?? string.Empty;
                cmd.Parameters.Add("@nvcKeyword", SqlDbType.NVarChar, 4000).Value = keyword ?? string.Empty;
                cmd.Parameters.Add("@bitKeywordAnd", SqlDbType.Bit).Value = bitKeywordAnd;
                cmd.Parameters.Add("@intMediaType", SqlDbType.Int).Value = codemediatype;
                cmd.Parameters.Add("@nvcRecipeName", SqlDbType.NVarChar, 4000).Value = recipename ?? string.Empty;
                cmd.Parameters.Add("@nvcRecipeNumber", SqlDbType.NVarChar, 4000).Value = recipenumber ?? string.Empty;
                cmd.Parameters.Add("@intRecipeNumberAnd", SqlDbType.Int).Value = recipenumberAnd;
                cmd.Parameters.Add("@Codetrans", SqlDbType.Int).Value = codetrans;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var dam = new List<Models.DigitalAssets>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var ext = GetStr(r["Extension"]);
                    if (string.Equals(ext, "JPG", StringComparison.OrdinalIgnoreCase) || string.Equals(ext, "PNG", StringComparison.OrdinalIgnoreCase))
                    {
                        dam.Add(new Models.DigitalAssets
                        {
                            id = GetInt(r["id"]),
                            ImageUrl = GetStr(r["ImageUrl"]),
                            MediaType = GetInt(r["MediaType"]),
                            FileName = GetStr(r["FileName"]),
                            Extension = ext,
                            Name = GetStr(r["Name"]),
                            Keyword = GetStr(r["Keyword"]) 
                        });
                    }
                }

                var totalCount = dam.Count;
                take = Math.Min(Math.Max(take, 1), totalCount);
                var totalPages = (int)Math.Ceiling(totalCount / (double)take);
                skip = Math.Min(Math.Max(skip, 0), totalPages);
                return Ok(new Models.ResponseDigitalAssets(dam.Skip(take * skip).Take(take).ToList(), totalCount));
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        private static int GetInt(object? value, int fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (int.TryParse(Convert.ToString(value), out var i)) return i;
            try { return Convert.ToInt32(value); } catch { return fallback; }
        }
        private static string GetStr(object? value)
        {
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
        }
    }

    // Placeholder models - replace with actual
    namespace Models
    {
        public class GenericCodeValueList { public int Code { get; set; } public string Value { get; set; } = string.Empty; }
        public class DigitalAssets { public int id { get; set; } public string ImageUrl { get; set; } = string.Empty; public int MediaType { get; set; } public string FileName { get; set; } = string.Empty; public string Extension { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public string Keyword { get; set; } = string.Empty; }
        public class ResponseDigitalAssets { public ResponseDigitalAssets(List<DigitalAssets> data, int total) { Data = data; Total = total; } public List<DigitalAssets> Data { get; set; } public int Total { get; set; } }
    }
}
