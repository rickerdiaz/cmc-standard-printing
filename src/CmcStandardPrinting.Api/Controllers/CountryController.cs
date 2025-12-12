using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CmcStandardPrinting.Domain.Units;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CountryController> _logger;

    public CountryController(IConfiguration configuration, ILogger<CountryController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codesite:int}/{codetrans:int}")]
    public ActionResult<List<Country>> GetCountryList(int codesite, int codetrans)
    {
        var countries = new List<Country>();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[GET_COUNRTYLIST]";
            cmd.Parameters.Add("@CodeSite", System.Data.SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", System.Data.SqlDbType.Int).Value = codetrans;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    countries.Add(new Country
                    {
                        Name = Convert.ToString(dr["Name"]) ?? string.Empty,
                        Code = Convert.ToInt32(dr["Code"]),
                        Abbr = Convert.ToString(dr["Abbr"]) ?? string.Empty
                    });
                }
            }
            dr.Close();
            return Ok(countries);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetCountryList: argument error");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCountryList: database error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }
}
