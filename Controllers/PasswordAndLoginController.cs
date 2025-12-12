using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordAndLoginController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet]
        public ActionResult<Models.PasswordAndLogin> GetPasswordAndLogin()
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Common.SP_API_GET_PasswordAndLoginInfo;
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();

                var passwordAndLogin = new Models.PasswordAndLogin
                {
                    EnforceStrongPassword = false,
                    ExpiresAfterNumberOfDays = string.Empty,
                    MinimumPasswordLength = 0,
                    MinimumPasswordReuse = string.Empty,
                    MaximumFailedLoginAttempts = 0,
                    LockoutPeriod = "0:m"
                };

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var r = ds.Tables[0].Rows[0];
                    passwordAndLogin.EnforceStrongPassword = Convert.ToBoolean(r["PasswordLoginEnforceStrongPolicy"]);
                    passwordAndLogin.ExpiresAfterNumberOfDays = Convert.ToString(r["PasswordLoginExpiresAfter"]);
                    passwordAndLogin.MinimumPasswordLength = Convert.ToInt32(r["PasswordLoginMinimumLength"]);
                    passwordAndLogin.MinimumPasswordReuse = Convert.ToString(r["PasswordLoginMinimumForReuse"]);
                    passwordAndLogin.MaximumFailedLoginAttempts = Convert.ToInt32(r["PasswordLoginMaximumFailedAttempts"]);
                    passwordAndLogin.LockoutPeriod = Convert.ToString(r["PasswordLoginLockoutPeriod"]);
                }

                return Ok(passwordAndLogin);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SaveAlias([FromBody] Models.PasswordAndLogin data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[API_UPDATE_PasswordAndLogin]";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@bitPasswordLoginEnforceStrongPolicy", SqlDbType.Bit).Value = data.EnforceStrongPassword;
                cmd.Parameters.Add("@strPasswordLoginExpiresAfter", SqlDbType.NVarChar).Value = data.ExpiresAfterNumberOfDays;
                cmd.Parameters.Add("@intPasswordLoginMinimumLength", SqlDbType.Int).Value = data.MinimumPasswordLength;
                cmd.Parameters.Add("@strPasswordLoginMinimumForReuse", SqlDbType.NVarChar).Value = data.MinimumPasswordReuse;
                cmd.Parameters.Add("@intPasswordLoginMaximumFailedAttempts", SqlDbType.Int).Value = data.MaximumFailedLoginAttempts;
                cmd.Parameters.Add("@strPasswordLoginLockoutPeriod", SqlDbType.NVarChar).Value = data.LockoutPeriod;
                cmd.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cn.Open();
                _trans = cn.BeginTransaction();
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Save Password and Login policies failed"));
                _trans.Commit();
                response.Code = 0; response.Message = "OK"; response.ReturnValue = string.Empty; response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                try { _trans?.Rollback(); } catch { }
                if (resultCode == 0) resultCode = 500;
                return StatusCode(500, Fail(response, resultCode, "Save Password and Login policies failed"));
            }
        }

        [HttpGet("alias/{codetrans:int}")]
        public ActionResult<List<Models.Alias>> SearchAlias(int codetrans)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Generic]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSEARCHALIASES";
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var aliases = new List<Models.Alias>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    aliases.Add(new Models.Alias
                    {
                        Code = GetInt(r["Code"]),
                        IdMain = GetInt(r["IdMain"]),
                        CodeTrans = GetInt(r["CodeTrans"]),
                        Name = GetStr(r["Name"]),
                        Alias = GetStr(r["Alias"]) 
                    });
                }
                return Ok(aliases);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("alias/{codetrans:int}/{name?}")]
        public ActionResult<List<Models.Alias>> SearchAliasByName(int codetrans, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(HttpContext.RequestServices.GetService<IConfiguration>()?.GetSection("AppSettings")?["dsn"] ?? string.Empty);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSEARCHALIASES";
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();
                var aliases = new List<Models.Alias>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    aliases.Add(new Models.Alias
                    {
                        Code = GetInt(r["Id"]),
                        IdMain = GetInt(r["IdMain"]),
                        CodeTrans = GetInt(r["CodeTrans"]),
                        Name = GetStr(r["Name"]),
                        Alias = GetStr(r["Alias"]) 
                    });
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var aliasresult = new List<Models.Alias>();
                    foreach (var word in name.Split(','))
                    {
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var a in aliases)
                        {
                            if (a.Name.ToLowerInvariant().Contains(w) || a.Alias.ToLowerInvariant().Contains(w))
                                aliasresult.Add(a);
                        }
                    }
                    aliases = aliasresult;
                }
                return Ok(aliases);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("alias/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteAlias([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var arrAliasCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrAliasCodes.Contains(c.Code)) arrAliasCodes.Add(c.Code);
                }
                var codeAliasList = Common.Join(arrAliasCodes, string.Empty, string.Empty, ",");
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeAliasList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWSEARCHALIASES";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Delete alias failed"));
                response.Code = 0; response.Message = "OK"; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value); response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode; response.Status = false; response.ReturnValue = string.Empty; response.Message = "Delete failed";
                return StatusCode(500, response);
            }
        }

        [HttpPost("alias/purge/{type:int}")]
        public ActionResult<Models.ResponseCallBack> PurgeAlias(int type, long codeUser, long codeSite)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_PURGE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWSEARCHALIASES";
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cn.Open();
                _trans = cn.BeginTransaction();
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Purge alias failed"));
                _trans.Commit();
                response.Code = 0; response.Message = "OK"; response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                try { _trans?.Rollback(); } catch { }
                if (resultCode == 0) resultCode = 500;
                return StatusCode(500, Fail(response, resultCode, "Purge alias failed"));
            }
        }

        // Helpers
        private static int GetInt(object value, int fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (int.TryParse(Convert.ToString(value), out var i)) return i; try { return Convert.ToInt32(value); } catch { return fallback; } }
        private static string GetStr(object value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
    }
}
