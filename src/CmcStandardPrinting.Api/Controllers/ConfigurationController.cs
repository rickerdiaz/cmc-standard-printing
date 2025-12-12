using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using CmcStandardPrinting.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(IConfiguration configuration, ILogger<ConfigurationController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codeuser:int}/{id:int?}")]
    public ActionResult<List<GenericListValue>> GetConfig(int codeuser, int id = -1)
    {
        var config = new List<GenericListValue>();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = Common.SP_API_GET_Config;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codeuser;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var raw = GetStr(dr["Value"]).ToUpperInvariant();
                var value = raw == "!B=1" ? bool.TrueString.ToLowerInvariant()
                           : raw == "!B=0" ? bool.FalseString.ToLowerInvariant()
                           : GetStr(dr["Value"]);
                config.Add(new GenericListValue { Code = GetInt(dr["Code"]), Value = value });
            }

            return Ok(config);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetConfig failed");
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
