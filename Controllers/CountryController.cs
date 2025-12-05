using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codesite:int}/{codetrans:int}")]
        public ActionResult<List<Models.Country>> GetCountryList(int codesite, int codetrans)
        {
            var countries = new List<Models.Country>();
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
                        countries.Add(new Models.Country
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
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }
    }

    // Placeholder model
    namespace Models
    {
        public class Country { public string Name { get; set; } = string.Empty; public int Code { get; set; } public string Abbr { get; set; } = string.Empty; }
    }
}
