using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeywordController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeywordController> _logger;

    public KeywordController(IConfiguration configuration, ILogger<KeywordController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codesite:int}/{codetrans:int}/{type:int}")]
    public ActionResult<List<GenericTree>> GetKeyword(int codesite, int codetrans, int type)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = Common.SP_GET_KEYWORDCODENAME;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@ListeType", SqlDbType.Int).Value = type;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var keywords = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                keywords.Add(new GenericTree
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    ParentCode = GetInt(r["ParentCode"]),
                    ParentName = GetStr(r["ParentName"]),
                    Flagged = false
                });
            }
            return Ok(keywords);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKeyword failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{codeproperty:int?}/{name?}")]
    public ActionResult<List<TreeNode>> GetKeywordByName(int codesite, int codetrans, int type, int tree, int codeliste, int codeproperty = -1, string? name = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Keywords]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name ?? string.Empty;
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var keywords = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                keywords.Add(new GenericTree
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    ParentCode = GetInt(r["ParentCode"]),
                    ParentName = GetStr(r["ParentName"]),
                    Flagged = GetBool(r["Flagged"]),
                    Note = GetStr(r["Inheritable"])
                });
            }

            var keyworddata = new List<TreeNode>();
            var parents = keywords.Where(obj => obj.ParentCode <= 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (keyworddata.All(obj => obj.Key != p.Code))
                {
                    var parent = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildren(keywords, p.Code),
                        Select = p.Flagged,
                        Selected = p.Flagged,
                        ParentTitle = p.ParentName,
                        Note = p.Note
                    };
                    keyworddata.Add(parent);
                }
            }
            return Ok(keyworddata);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKeywordByName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("search")]
    public ActionResult<List<TreeNode>> GetKeywordByName2([FromBody] ConfigurationcSearch data)
    {
        try
        {
            data.Name = string.IsNullOrEmpty(data.Name) || data.Name == "null" || data.Name == "undefined" ? string.Empty : data.Name;
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Keywords]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = data.Type;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = data.CodeListe;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = data.Name;
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var keywords = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                keywords.Add(new GenericTree
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    ParentCode = GetInt(r["ParentCode"]),
                    ParentName = GetStr(r["ParentName"]),
                    Flagged = GetBool(r["Flagged"]),
                    Note = GetStr(r["Inheritable"])
                });
            }

            var keyworddata = new List<TreeNode>();
            var parents = keywords.Where(obj => obj.ParentCode <= 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (keyworddata.All(obj => obj.Key != p.Code))
                {
                    var parent = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildren(keywords, p.Code),
                        Select = p.Flagged,
                        Selected = p.Flagged,
                        ParentTitle = p.ParentName,
                        Note = p.Note
                    };
                    keyworddata.Add(parent);
                }
            }
            return Ok(keyworddata);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKeywordByName2 failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{codeproperty:int?}")]
    public ActionResult<List<TreeNode>> SearchKeywordByName(int codesite, int codetrans, int type, int tree, int codeliste, int codeproperty, [FromBody] string? name)
    {
        return GetKeywordByName(codesite, codetrans, type, tree, codeliste, codeproperty, name);
    }

    private List<TreeNode> CreateChildren(List<GenericTree> data, int parent)
    {
        var children = new List<TreeNode>();
        foreach (var k in data.Where(obj => obj.ParentCode == parent).OrderBy(obj => obj.Name))
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = null,
                Select = k.Flagged,
                Selected = k.Flagged,
                ParentTitle = k.ParentName,
                GroupLevel = GroupLevel.Site,
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
}
