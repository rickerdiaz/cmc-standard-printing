using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using CmcStandardPrinting.Domain.Sites;
using CmcStandardPrinting.Domain.Units;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class TaxController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TaxController> _logger;

    public TaxController(IConfiguration configuration, ILogger<TaxController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("/api/tax/{codesite:int}")]
    public ActionResult<List<Tax>> GetTax(int codesite)
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
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWTAX";
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var taxes = new List<Tax>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                taxes.Add(new Tax
                {
                    TaxCode = GetInt(r["Code"]),
                    TaxValue = GetDbl(r["Value"]),
                    TaxName = GetStr(r["Description"]),
                    Global = GetBool(r["Global"])
                });
            }
            return Ok(taxes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTax: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/tax/{codesite:int}/{type:int}/{codeproperty:int?}/{name?}")]
    public ActionResult<List<Tax>> GetTaxByName(int codesite, int type, int? codeproperty = -1, string name = "")
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
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWTAX";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty ?? -1;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var taxes = MapTaxes(ds.Tables[0]);
            taxes = FilterByName(taxes, name);
            return Ok(taxes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTaxByName: database error");
            return StatusCode(500);
        }
    }

    [HttpPost("/api/tax/search")]
    public ActionResult<List<Tax>> GetTaxByName2([FromBody] ConfigurationSearch data)
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
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWTAX";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var taxes = MapTaxes(ds.Tables[0]);
            taxes = FilterByName(taxes, data.Name ?? string.Empty);
            return Ok(taxes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTaxByName2: database error");
            return StatusCode(500);
        }
    }

    private static List<Tax> MapTaxes(DataTable table)
    {
        var taxes = new List<Tax>();
        foreach (DataRow r in table.Rows)
        {
            taxes.Add(new Tax
            {
                TaxCode = GetInt(r["Code"]),
                TaxValue = GetDbl(r["Value"]),
                TaxName = GetStr(r["Description"]),
                Global = GetBool(r["Global"])
            });
        }
        return taxes;
    }

    private static List<Tax> FilterByName(List<Tax> taxes, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return taxes;

        var results = new List<Tax>();
        foreach (var w in name.Split(','))
        {
            var word = (w ?? string.Empty).Trim();
            if (word.Length == 0) continue;
            var key = ReplaceSpecialCharacters(word.ToLowerInvariant());
            foreach (var s in taxes)
            {
                if (s.TaxValue.ToString(CultureInfo.InvariantCulture).ToLowerInvariant().Contains(key) ||
                    (!string.IsNullOrEmpty(s.TaxName) && s.TaxName.ToLowerInvariant().Contains(key)))
                {
                    results.Add(s);
                }
            }
        }

        return results;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
        try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
    }

    private static double GetDbl(object? value, double fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is double d) return d;
        if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var dd)) return dd;
        try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); } catch { return fallback; }
    }

    private static string GetStr(object? value, string fallback = "")
    {
        if (value == null || value == DBNull.Value) return fallback;
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var result = value;
        var specialChars = new Dictionary<string, string>
        {
            { "ä", "ae" },
            { "ö", "oe" },
            { "ü", "ue" },
            { "ß", "ss" },
            { "é", "e" },
            { "è", "e" },
            { "ê", "e" },
            { "à", "a" },
            { "á", "a" },
            { "â", "a" },
            { "ù", "u" },
            { "û", "u" },
            { "ú", "u" },
            { "î", "i" },
            { "ï", "i" }
        };

        foreach (var k in specialChars.Keys)
        {
            result = result.Replace(k, specialChars[k], StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}
