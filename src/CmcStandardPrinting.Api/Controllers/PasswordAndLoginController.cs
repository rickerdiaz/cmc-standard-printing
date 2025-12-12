using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Linq;
using CmcStandardPrinting.Domain.Aliases;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordAndLoginController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordAndLoginController> _logger;

    public PasswordAndLoginController(IConfiguration configuration, ILogger<PasswordAndLoginController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet]
    public ActionResult<PasswordAndLogin> GetPasswordAndLogin()
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString),
                CommandType = CommandType.StoredProcedure,
                CommandText = Common.SP_API_GET_PasswordAndLoginInfo
            };

            cmd.Connection.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }

            var passwordAndLogin = new PasswordAndLogin
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
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetPasswordAndLogin failed");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPasswordAndLogin failed");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SavePasswordAndLogin([FromBody] PasswordAndLogin data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[API_UPDATE_PasswordAndLogin]";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@bitPasswordLoginEnforceStrongPolicy", SqlDbType.Bit).Value = data.EnforceStrongPassword;
            cmd.Parameters.Add("@strPasswordLoginExpiresAfter", SqlDbType.NVarChar).Value = data.ExpiresAfterNumberOfDays ?? string.Empty;
            cmd.Parameters.Add("@intPasswordLoginMinimumLength", SqlDbType.Int).Value = data.MinimumPasswordLength;
            cmd.Parameters.Add("@strPasswordLoginMinimumForReuse", SqlDbType.NVarChar).Value = data.MinimumPasswordReuse ?? string.Empty;
            cmd.Parameters.Add("@intPasswordLoginMaximumFailedAttempts", SqlDbType.Int).Value = data.MaximumFailedLoginAttempts;
            cmd.Parameters.Add("@strPasswordLoginLockoutPeriod", SqlDbType.NVarChar).Value = data.LockoutPeriod ?? string.Empty;
            cmd.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
            if (resultCode != 0)
            {
                return StatusCode(500, Fail(response, resultCode, "Save Password and Login policies failed"));
            }

            trans.Commit();
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = string.Empty;
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            try
            {
                trans?.Rollback();
            }
            catch
            {
                // ignored
            }

            _logger.LogError(ex, "SavePasswordAndLogin failed");
            if (resultCode == 0)
            {
                resultCode = 500;
            }

            return StatusCode(500, Fail(response, resultCode, "Save Password and Login policies failed"));
        }
    }

    [HttpGet("alias/{codetrans:int}")]
    public ActionResult<List<AliasInfo>> SearchAlias(int codetrans)
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
            var aliases = new List<AliasInfo>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    aliases.Add(new AliasInfo
                    {
                        Code = GetInt(r["Code"]),
                        IdMain = GetInt(r["IdMain"]),
                        CodeTrans = GetInt(r["CodeTrans"]),
                        Name = GetString(r, "Name"),
                        Alias = GetString(r, "Alias")
                    });
                }
            }

            return Ok(aliases);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "SearchAlias failed");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchAlias failed");
            return StatusCode(500);
        }
    }

    [HttpGet("alias/{codetrans:int}/{name?}")]
    public ActionResult<List<AliasInfo>> SearchAliasByName(int codetrans, string? name = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand
            {
                Connection = new SqlConnection(_configuration.GetSection("AppSettings")?["dsn"] ?? string.Empty),
                CommandType = CommandType.StoredProcedure,
                CommandText = "[dbo].[API_MANAGE_Generic]"
            };

            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSEARCHALIASES";
            cmd.Connection.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }

            var aliases = new List<AliasInfo>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    aliases.Add(new AliasInfo
                    {
                        Code = GetInt(r["Id"]),
                        IdMain = GetInt(r["IdMain"]),
                        CodeTrans = GetInt(r["CodeTrans"]),
                        Name = GetString(r, "Name"),
                        Alias = GetString(r, "Alias")
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var aliasresult = new List<AliasInfo>();
                foreach (var word in name.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var w = ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                    aliasresult.AddRange(aliases.Where(a => a.Name.ToLowerInvariant().Contains(w) || a.Alias.ToLowerInvariant().Contains(w)));
                }

                aliases = aliasresult;
            }

            return Ok(aliases);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "SearchAliasByName failed");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchAliasByName failed");
            return StatusCode(500);
        }
    }

    [HttpPost("alias/delete")]
    public ActionResult<ResponseCallBack> DeleteAlias([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            var aliasCodes = data.CodeList
                .Select(c => c.Code)
                .Distinct()
                .ToArray();
            var codeAliasList = string.Join(',', aliasCodes);

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
            if (resultCode != 0)
            {
                return StatusCode(500, Fail(response, resultCode, "Delete alias failed"));
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetString(cmd.Parameters["@SkipList"].Value);
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAlias failed");
            if (resultCode == 0)
            {
                resultCode = 500;
            }

            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Delete failed";
            return StatusCode(500, response);
        }
    }

    [HttpPost("alias/purge/{type:int}")]
    public ActionResult<ResponseCallBack> PurgeAlias(int type, long codeUser, long codeSite)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        SqlTransaction? trans = null;
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
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
            if (resultCode != 0)
            {
                return StatusCode(500, Fail(response, resultCode, "Purge alias failed"));
            }

            trans.Commit();
            response.Code = 0;
            response.Message = "OK";
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PurgeAlias failed");
            try
            {
                trans?.Rollback();
            }
            catch
            {
                // ignored
            }

            if (resultCode == 0)
            {
                resultCode = 500;
            }

            return StatusCode(500, Fail(response, resultCode, "Purge alias failed"));
        }
    }

    private static ResponseCallBack Fail(ResponseCallBack response, int code, string message)
    {
        response.Code = code;
        response.Message = message;
        response.Status = false;
        response.ReturnValue = string.Empty;
        return response;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value)
        {
            return fallback;
        }

        if (int.TryParse(Convert.ToString(value), out var i))
        {
            return i;
        }

        try
        {
            return Convert.ToInt32(value);
        }
        catch
        {
            return fallback;
        }
    }

    private static string GetString(object value, string? name = null)
    {
        if (name is null)
        {
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
        }

        if (value is DataRow row)
        {
            return row.IsNull(name) ? string.Empty : Convert.ToString(row[name]) ?? string.Empty;
        }

        return string.Empty;
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        var replacements = new[]
        {
            new { Key = "Ä", Value = "AE" },
            new { Key = "Ö", Value = "OE" },
            new { Key = "Ü", Value = "UE" },
            new { Key = "ä", Value = "ae" },
            new { Key = "ö", Value = "oe" },
            new { Key = "ü", Value = "ue" },
            new { Key = "ß", Value = "ss" },
            new { Key = "ç", Value = "c" },
            new { Key = "ñ", Value = "n" }
        };

        return replacements.Aggregate(value, (current, replacement) => current.Replace(replacement.Key, replacement.Value));
    }
}
