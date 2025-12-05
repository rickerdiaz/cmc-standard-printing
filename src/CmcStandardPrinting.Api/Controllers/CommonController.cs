using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommonController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CommonController> _logger;

    public CommonController(IConfiguration configuration, ILogger<CommonController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("autonumber/{codesite:int}/{codeuser:int}/{type:int}/{category:int?}")]
    public ActionResult<ResponseCallBack> GetAutoNumber(int codesite, int codeuser, int type, int category = -1)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            string strNumber;
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_AutoNumber]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = category;
            var pNumber = cmd.Parameters.Add("@Number", SqlDbType.VarChar, 50);
            pNumber.Direction = ParameterDirection.Output;
            var pErr = cmd.Parameters.Add("@ERR", SqlDbType.Int);
            pErr.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            strNumber = Convert.ToString(pNumber.Value) ?? string.Empty;
            resultCode = Convert.ToInt32(pErr.Value);
            if (resultCode != 0)
            {
                throw new Exception($"[{resultCode}] Get Autonumber failed");
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = strNumber;
            response.Status = true;
        }
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Get Autonumber failed";
            LogError("GetAutoNumber: Database error occured", ex);
            return StatusCode(500, response);
        }

        return Ok(response);
    }

    [HttpGet("sharing/{codeegstable:int}/{codesite:int}/{code:int}")]
    public ActionResult<List<TreeNode>> GetSharing(int codeegstable, int codesite, int code)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "[dbo].[API_GET_SharingAll]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = codeegstable;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var sharings = new List<GenericTree>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Type = GetInt(r["Type"]),
                        Global = GetBool(r["Global"])
                    });
                }
            }

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new TreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = CreateChildrenSharing(sharings, p.Code),
                    Select = p.Flagged,
                    Selected = p.Flagged,
                    ParentTitle = p.ParentName,
                    GroupLevel = GroupLevel.Property
                };
                if (parent.Children != null && parent.Children.Count > 0)
                {
                    sharingdata.Add(parent);
                }
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException aex)
        {
            LogWarn($"GetSharing: Missing or invalid parameters ({codeegstable})", aex);
            return Problem(title: $"Request failed ({codeegstable})", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError($"GetSharing: Unexpected error ({codeegstable})", ex);
            return Problem(title: $"Request failed ({codeegstable})", statusCode: 500);
        }
    }

    [HttpGet("translation/{codeegstable:int}/{codesite:int}/{code:int}")]
    public ActionResult<List<GenericTranslation>> GetTranslation(int codeegstable, int codesite, int code)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_TranslationAll";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
            cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = codeegstable;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var translations = new List<GenericTranslation>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    translations.Add(new GenericTranslation
                    {
                        CodeTrans = GetInt(r["CodeTrans"]),
                        TranslationName = GetStr(r["TranslationName"]),
                        Name = GetStr(r["Name"])
                    });
                }
            }

            return Ok(translations);
        }
        catch (ArgumentException aex)
        {
            LogWarn($"GetTranslation: Missing or invalid parameters ({codeegstable})", aex);
            return Problem(title: $"Request failed ({codeegstable})", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError($"GetTranslation: Unexpected error ({codeegstable})", ex);
            return Problem(title: $"Request failed ({codeegstable})", statusCode: 500);
        }
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> sharings, int code)
    {
        var children = new List<TreeNode>();
        var kids = sharings.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = null!,
                Select = k.Flagged,
                Selected = k.Flagged,
                ParentTitle = k.ParentName,
                GroupLevel = GroupLevel.Property,
                Note = k.Global
            };
            children.Add(child);
        }
        return children;
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

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (bool.TryParse(Convert.ToString(value), out var b)) return b;
        try { return Convert.ToInt32(value) != 0; } catch { return false; }
    }

    private void LogWarn(string msg, Exception ex) => _logger.LogWarning(ex, msg);
    private void LogError(string msg, Exception ex) => _logger.LogError(ex, msg);
}
