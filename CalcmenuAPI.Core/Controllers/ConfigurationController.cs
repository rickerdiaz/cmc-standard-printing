using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using log4net;
using Microsoft.AspNetCore.Mvc;
using static EgsData.modGlobalDeclarations; // ConnectionString
using static EgsData.modFunctions;         // GetInt, GetStr

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpGet("/api/config/{codeuser:int?}/{id:int?}")]
        public ActionResult<List<Models.GenericList>> GetConfig(int codeuser, int id = -1)
        {
            var config = new List<Models.GenericList>();
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = Common.SP_API_GET_Config;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = GetInt(codeuser);
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = GetInt(id, -1);
                    cn.Open();
                    using SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var value = GetStr(dr["Value"]).ToUpper();
                        string normalized = value == "!B=1" ? bool.TrueString.ToLower() : value == "!B=0" ? bool.FalseString.ToLower() : GetStr(dr["Value"]);
                        config.Add(new Models.GenericList
                        {
                            Code = GetInt(dr["Code"]),
                            Value = normalized
                        });
                    }
                    dr.Close();
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

                return Ok(config);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }
    }
}
