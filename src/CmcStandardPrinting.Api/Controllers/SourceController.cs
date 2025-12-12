using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Linq;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Sources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SourceController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SourceController> _logger;

    public SourceController(IConfiguration configuration, ILogger<SourceController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codesite:int}")]
    public ActionResult<List<GenericCodeValueList>> GetSource(int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSOURCE";
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<GenericCodeValueList>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new GenericCodeValueList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
            }
            return Ok(list);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSource failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("search")]
    public ActionResult<List<Source>> GetSourceByName2([FromBody] ConfigurationcSearch data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSOURCE";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var sources = new List<Source>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sources.Add(new Source
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"])
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var results = new List<Source>();
                foreach (var w in data.Name.Split(','))
                {
                    var word = (w ?? string.Empty).Trim();
                    if (word.Length == 0) continue;
                    var key = ReplaceSpecialCharacters(word.ToLowerInvariant());
                    foreach (var s in sources)
                    {
                        if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                    }
                }
                sources = results;
            }

            return Ok(sources);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSourceByName2 failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{type:int}/{codeproperty:int?}/{name?}")]
    public ActionResult<List<Source>> GetSourceByName(int codesite, int type, int? codeproperty = -1, string name = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSOURCE";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty ?? -1;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<Source>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Source
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"])
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var results = new List<Source>();
                foreach (var w in name.Split(','))
                {
                    var word = (w ?? string.Empty).Trim();
                    if (word.Length == 0) continue;
                    var key = ReplaceSpecialCharacters(word.ToLowerInvariant());
                    foreach (var s in list)
                    {
                        if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                    }
                }
                list = results;
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSourceByName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        return value.Replace("é", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("ä", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("ö", "o", StringComparison.OrdinalIgnoreCase)
            .Replace("ü", "u", StringComparison.OrdinalIgnoreCase)
            .Replace("è", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("à", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("ò", "o", StringComparison.OrdinalIgnoreCase)
            .Replace("ù", "u", StringComparison.OrdinalIgnoreCase)
            .Replace("ç", "c", StringComparison.OrdinalIgnoreCase)
            .Replace("â", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("ê", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("î", "i", StringComparison.OrdinalIgnoreCase)
            .Replace("ô", "o", StringComparison.OrdinalIgnoreCase)
            .Replace("û", "u", StringComparison.OrdinalIgnoreCase)
            .Replace("ë", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("ï", "i", StringComparison.OrdinalIgnoreCase)
            .Replace("ü", "u", StringComparison.OrdinalIgnoreCase)
            .Replace("ä", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("ö", "o", StringComparison.OrdinalIgnoreCase)
            .Replace("á", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("é", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("í", "i", StringComparison.OrdinalIgnoreCase)
            .Replace("ó", "o", StringComparison.OrdinalIgnoreCase)
            .Replace("ú", "u", StringComparison.OrdinalIgnoreCase)
            .Replace("ñ", "n", StringComparison.OrdinalIgnoreCase)
            .Replace("ã", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("õ", "o", StringComparison.OrdinalIgnoreCase);
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

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (bool.TryParse(Convert.ToString(value), out var b)) return b;
        try { return Convert.ToInt32(value) != 0; } catch { return false; }
    }
}
