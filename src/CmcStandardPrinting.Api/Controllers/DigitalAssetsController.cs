using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CmcStandardPrinting.Domain.DigitalAssets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DigitalAssetsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DigitalAssetsController> _logger;

    public DigitalAssetsController(IConfiguration configuration, ILogger<DigitalAssetsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("imagetype")]
    public ActionResult<List<GenericCodeValueList>> GetImageType()
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

            var imagetype = new List<GenericCodeValueList>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    imagetype.Add(new GenericCodeValueList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
            }

            return Ok(imagetype);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetImageType failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{codeuser:int}/{codemediatype:int}/{bitKeywordAnd:bool}/{name?}")]
    public ActionResult<ResponseDigitalAssets> GetDigitalAssets(
        int codesite,
        int codetrans,
        int codeuser,
        int codemediatype,
        bool bitKeywordAnd,
        string? name = "",
        string? keyword = "",
        string? recipenumber = "",
        int recipenumberAnd = 0,
        string? recipename = "",
        int take = 10,
        int skip = 0)
    {
        try
        {
            name = DecodeBase64IfNeeded(name);
            keyword = DecodeBase64IfNeeded(keyword);
            recipename = DecodeBase64IfNeeded(recipename);
            recipenumber = DecodeBase64IfNeeded(recipenumber);

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

            var assets = new List<DigitalAsset>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var ext = GetStr(r["Extension"]);
                    if (!string.Equals(ext, "JPG", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(ext, "PNG", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    assets.Add(new DigitalAsset
                    {
                        Id = GetInt(r["id"]),
                        ImageUrl = GetStr(r["ImageUrl"]),
                        MediaType = GetInt(r["MediaType"]),
                        FileName = GetStr(r["FileName"]),
                        Extension = ext,
                        Name = GetStr(r["Name"]),
                        Keyword = GetStr(r["Keyword"])
                    });
                }
            }

            var totalCount = assets.Count;
            take = Math.Min(Math.Max(take, 1), Math.Max(totalCount, 1));
            var totalPages = (int)Math.Ceiling(totalCount / (double)take);
            skip = Math.Min(Math.Max(skip, 0), Math.Max(totalPages - 1, 0));

            return Ok(new ResponseDigitalAssets
            {
                Data = assets.Skip(take * skip).Take(take).ToList(),
                Total = totalCount
            });
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDigitalAssets failed");
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

    private static string? DecodeBase64IfNeeded(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        try
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(value));
        }
        catch
        {
            return value;
        }
    }
}
