using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Globalization;
using CmcStandardPrinting.Domain.Sites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PropertyController> _logger;

    public PropertyController(IConfiguration configuration, ILogger<PropertyController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet]
    public ActionResult<List<GenericListItem>> GetProperty()
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Properties]";
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var properties = new List<GenericListItem>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    properties.Add(new GenericListItem
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["Name"])
                    });
                }
            }

            return Ok(properties);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetProperty: Missing or invalid parameters", aex);
            return BadRequest();
        }
        catch (Exception ex)
        {
            LogError("GetProperty: Unexpected error occurred", ex);
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

    private void LogWarn(string msg, Exception ex) => _logger.LogWarning(ex, msg);
    private void LogError(string msg, Exception ex) => _logger.LogError(ex, msg);
}
