using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codeuser:int}/{id:int?}")]
        public ActionResult<List<Models.GenericList>> GetConfig(int codeuser, int id = -1)
        {
            var config = new List<Models.GenericList>();
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
                    var value = raw == "!B=1" ? true.ToString().ToLowerInvariant()
                               : raw == "!B=0" ? false.ToString().ToLowerInvariant()
                               : GetStr(dr["Value"]);
                    config.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = value });
                }
                dr.Close();
                return Ok(config);
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

    // Placeholder models & Common
    namespace Models
    {
        public class GenericList { public int Code { get; set; } public string Value { get; set; } = string.Empty; }
    }
    public static class Common
    {
        public const string SP_API_GET_Config = "API_GET_Config";
    }
}
