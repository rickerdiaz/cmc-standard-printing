using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserController> _logger;

    public UserController(IConfiguration configuration, ILogger<UserController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("/api/userlist/{codesite:int}")]
    public ActionResult<List<GenericListValue>> GetUserList(int codesite)
    {
        try
        {
            var users = new List<GenericListValue>();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Users]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            da.Fill(ds);
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                users.Add(new GenericListValue
                {
                    Code = GetInt(r["Code"]),
                    Value = GetStr(r["Name"])
                });
            }

            return Ok(users);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetUserList: invalid argument");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserList: unexpected error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("/api/userlist/nonadmin/{codesite:int}")]
    public ActionResult<List<GenericListValue>> GetNonAdminUser(int codesite)
    {
        try
        {
            var users = new List<GenericListValue>();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_UsersNonAdmin]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            da.Fill(ds);
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                users.Add(new GenericListValue
                {
                    Code = GetInt(r["Code"]),
                    Value = GetStr(r["Name"])
                });
            }

            return Ok(users);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetNonAdminUser: invalid argument");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetNonAdminUser: unexpected error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("/api/user/rights/{codeuser:int}")]
    public ActionResult<string> GetUserRights(int codeuser)
    {
        try
        {
            var list = new List<UserRights>();
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
                list.Add(new UserRights
                {
                    RoleId = GetInt(dr["RoleId"]),
                    Name = GetStr(dr["Name"]),
                    RoleLevel = GetInt(dr["RoleLevel"]),
                    Modules = GetInt(dr["Modules"]),
                    Rights = GetInt(dr["Rights"])
                });
            }

            var json = JsonConvert.SerializeObject(list, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return Ok(json);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetUserRights: invalid argument");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserRights: unexpected error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("/api/user/{codeuser:int}/")]
    public ActionResult<UserData> GetUserInfo(int codeuser)
    {
        try
        {
            var data = new UserData();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_UserInfo]";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count == 3)
            {
                var tblInfo = ds.Tables[0];
                var tblConfig = ds.Tables[1];
                var tblRights = ds.Tables[2];

                if (tblInfo.Rows.Count > 0)
                {
                    var info = new User();
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

                    data.Info = JsonConvert.SerializeObject(info, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                if (tblConfig.Rows.Count > 0)
                {
                    var config = new List<GenericListValue>();
                    foreach (DataRow dr in tblConfig.Rows)
                    {
                        config.Add(new GenericListValue
                        {
                            Code = GetInt(dr["Code"]),
                            Value = GetStr(dr["Value"])
                        });
                    }

                    data.Config = JsonConvert.SerializeObject(config, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }

                if (tblRights.Rows.Count > 0)
                {
                    var rights = new List<GenericListValue>();
                    foreach (DataRow dr in tblRights.Rows)
                    {
                        rights.Add(new GenericListValue
                        {
                            Code = GetInt(dr["Modules"]),
                            Value = GetStr(dr["Rights"])
                        });
                    }

                    data.Rights = JsonConvert.SerializeObject(rights, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
            }

            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetUserInfo: invalid argument");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserInfo: unexpected error");
            return Problem(title: "Request failed", statusCode: 500);
        }
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
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "ClearPrinterQueue: invalid argument");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ClearPrinterQueue: unexpected error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private static int GetInt(object value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
        try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); }
        catch { return fallback; }
    }

    private static string GetStr(object value, string fallback = "")
    {
        if (value == null || value == DBNull.Value) return fallback;
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }
}
