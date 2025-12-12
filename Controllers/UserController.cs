using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CalcmenuAPI
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/userlist/{codesite:int}")]
        public ActionResult<List<Models.GenericList>> GetUserList(int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Users]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var users = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    users.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["Name"]) 
                    });
                }
                return Ok(users);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/userlist/nonadmin/{codesite:int}")]
        public ActionResult<List<Models.GenericList>> GetNonAdminUser(int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_UsersNonAdmin]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var users = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    users.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["Name"]) 
                    });
                }
                return Ok(users);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/user/rights/{codeuser:int}")]
        public ActionResult<string> GetUserRights(int codeuser)
        {
            try
            {
                var list = new List<Models.UserRights>();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "API_GET_UserRole";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                cn.Open();
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Models.UserRights
                    {
                        RoleId = GetInt(dr["RoleId"]),
                        Name = GetStr(dr["Name"]),
                        RoleLevel = GetInt(dr["RoleLevel"]),
                        Modules = GetInt(dr["Modules"]),
                        Rights = GetInt(dr["Rights"]) 
                    });
                }
                var json = JsonConvert.SerializeObject(list, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return Ok(json);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/user/{codeuser:int}/")]
        public ActionResult<Models.UserData> GetUserInfo(int codeuser)
        {
            try
            {
                var data = new Models.UserData();
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_UserInfo]";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds != null && ds.Tables.Count == 3)
                {
                    var tblInfo = ds.Tables[0];
                    var tblConfig = ds.Tables[1];
                    var tblRights = ds.Tables[2];

                    var info = new Models.User();
                    foreach (DataRow dr in tblInfo.Rows)
                    {
                        info.Code = GetInt(dr["Code"]);
                        info.UserName = GetStr(dr["UserName"]);
                        info.Name = GetStr(dr["Name"]);
                        info.Email = GetStr(dr["Email"]);
                        info.CodeSite = GetInt(dr["CodeSite"]);
                        info.RoleLevel = GetInt(dr["RoleLevel"]);
                        info.SalesSite = GetInt(dr["SalesSite"]);
                        info.SalesSiteLanguage = GetInt(dr["SalesSiteLanguage"]);
                        info.SiteName = GetStr(dr["SiteName"]);
                    }
                    data.Info = JsonConvert.SerializeObject(info, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    if (tblConfig.Rows.Count > 0)
                    {
                        var config = new List<Models.GenericList>();
                        foreach (DataRow dr in tblConfig.Rows)
                        {
                            config.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]) });
                        }
                        data.Config = JsonConvert.SerializeObject(config, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }

                    if (tblRights.Rows.Count > 0)
                    {
                        var rights = new List<Models.GenericList>();
                        foreach (DataRow dr in tblRights.Rows)
                        {
                            rights.Add(new Models.GenericList { Code = GetInt(dr["Modules"]), Value = GetStr(dr["Rights"]) });
                        }
                        data.Rights = JsonConvert.SerializeObject(rights, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }
                }

                return Ok(data);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/user/clearprinterqueue/")]
        public ActionResult<int> ClearPrinterQueue()
        {
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM EgswLabeltoPrint";
                cn.Open();
                cmd.ExecuteNonQuery();
                return Ok(1);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers
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
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
