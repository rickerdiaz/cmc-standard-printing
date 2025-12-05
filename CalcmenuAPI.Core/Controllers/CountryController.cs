using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using log4net;
using Microsoft.AspNetCore.Mvc;
using static EgsData.modGlobalDeclarations; // ConnectionString
using static EgsData.modFunctions;         // GetInt, GetStr

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class CountryController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpGet("/api/country/{codesite:int}/{codetrans:int}")]
        public ActionResult<List<Models.Country>> GetCountryList(int codesite, int codetrans)
        {
            var countries = new List<Models.Country>();
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_COUNRTYLIST]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var country = new Models.Country
                        {
                            Name = GetStr(dr["Name"]),
                            Code = GetInt(dr["Code"]),
                            Abbr = GetStr(dr["Abbr"]) 
                        };
                        countries.Add(country);
                    }
                }
                dr.Close();
                return Ok(countries);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 400);
            }
        }
    }
}
