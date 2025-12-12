using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Texts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TextController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TextController> _logger;

    public TextController(IConfiguration configuration, ILogger<TextController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("search")]
    public ActionResult<List<Text>> GetTextByName2([FromBody] ConfigurationcSearch data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_SEARCH_TEXT]";
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.Name ?? string.Empty;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int, 50).Value = 10;
            cmd.Parameters.Add("@namefiltertype", SqlDbType.Int, 10).Value = 4;
            cmd.Parameters.Add("@skip", SqlDbType.Int, 0).Value = 0;

            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var texts = new List<Text>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                texts.Add(new Text
                {
                    TextCode = GetInt(r["Code"]),
                    TextName = GetStr(r["Name"]),
                    TextDate = GetBool(r["Dates"]).ToString(),
                });
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                // Legacy behavior returns an empty list when a name filter is provided.
                texts = new List<Text>();
            }

            return Ok(texts);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Missing or invalid parameters for text search");
            return BadRequest();
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error during text search");
            return StatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during text search");
            return StatusCode(500);
        }
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        return Convert.ToInt32(value);
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        return Convert.ToBoolean(value);
    }

    private static string GetStr(object? value)
    {
        if (value == null || value == DBNull.Value) return string.Empty;
        return value.ToString() ?? string.Empty;
    }
}
