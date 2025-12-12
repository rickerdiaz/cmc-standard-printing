using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CmcStandardPrinting.Domain.RssFeeds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RssFeedStatsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RssFeedStatsController> _logger;

    public RssFeedStatsController(IConfiguration configuration, ILogger<RssFeedStatsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet]
    public ActionResult<RssFeedStats> GetRecipe()
    {
        var stats = new RssFeedStats();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_GetRSSFeedsStats]";
            cn.Open();

            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    stats.StatsOverview = GetStr(dr["Overview"]);
                }
            }

            dr.NextResult();
            if (dr.HasRows)
            {
                var perYear = new List<StatsPerYear>();
                while (dr.Read())
                {
                    perYear.Add(new StatsPerYear
                    {
                        DetailsPerYear = GetStr(dr["DetailsPerYear"])
                    });
                }
                stats.StatsPerYear = perYear;
            }

            dr.NextResult();
            if (dr.HasRows)
            {
                var perMonth = new List<StatsPerMonth>();
                while (dr.Read())
                {
                    perMonth.Add(new StatsPerMonth
                    {
                        DetailsPerMonth = GetStr(dr["DetailsPerMonth"])
                    });
                }
                stats.StatsPerMonth = perMonth;
            }

            dr.NextResult();
            if (dr.HasRows)
            {
                var perDay = new List<StatsPerDay>();
                while (dr.Read())
                {
                    perDay.Add(new StatsPerDay
                    {
                        DetailsPerDay = GetStr(dr["DetailsPerDay"])
                    });
                }
                stats.StatsPerDay = perDay;
            }

            dr.NextResult();
            if (dr.HasRows)
            {
                var perHour = new List<StatsPerHours>();
                while (dr.Read())
                {
                    perHour.Add(new StatsPerHours
                    {
                        DetailsPerHours = GetStr(dr["DetailsPerHour"])
                    });
                }
                stats.StatsPerHour = perHour;
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Missing or invalid parameters for RSS feed stats");
            return BadRequest();
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error while fetching RSS feed stats");
            return StatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching RSS feed stats");
            return StatusCode(500);
        }

        return Ok(stats);
    }

    private static string GetStr(object? value)
    {
        if (value == null || value == DBNull.Value) return string.Empty;
        return value.ToString() ?? string.Empty;
    }
}
