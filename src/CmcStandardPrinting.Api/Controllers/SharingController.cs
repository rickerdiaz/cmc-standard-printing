using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharingController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SharingController> _logger;

    public SharingController(IConfiguration configuration, ILogger<SharingController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}")]
    public ActionResult<List<TreeNode>> GetSharing(int codesite, int codetrans, int type, int tree, int codeliste)
    {
        try
        {
            _ = tree;
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Sharing]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cn.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }

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
                        Type = GetInt(r["Type"])
                    });
                }
            }

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
            foreach (var parentNode in parents)
            {
                var parent = new TreeNode
                {
                    Title = parentNode.Name,
                    Key = parentNode.Code,
                    Unselectable = parentNode.Code == codesite,
                    AddClass = parentNode.Code == codesite ? "main" : null,
                    Children = CreateChildren(sharings, parentNode.Code, codesite),
                    Select = parentNode.Flagged,
                    Selected = parentNode.Flagged,
                    ParentTitle = parentNode.ParentName,
                    GroupLevel = GroupLevel.Property
                };
                sharingdata.Add(parent);
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetSharing: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetSharing: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private static List<TreeNode> CreateChildren(List<GenericTree> sharingdata, int code, int codesite)
    {
        var children = new List<TreeNode>();
        if (sharingdata.Count == 0)
        {
            return children;
        }

        var kids = sharingdata.Where(o => o.ParentCode == code && o.Type == 2).OrderBy(o => o.Name).ToList();
        foreach (var childNode in kids)
        {
            var child = new TreeNode
            {
                Title = childNode.Name,
                Key = childNode.Code,
                Unselectable = childNode.Code == codesite,
                AddClass = childNode.Code == codesite ? "main" : null,
                Select = childNode.Code == codesite ? true : childNode.Flagged,
                Selected = childNode.Code == codesite ? true : childNode.Flagged,
                Children = new List<TreeNode>(),
                ParentTitle = childNode.ParentName,
                GroupLevel = GroupLevel.Site
            };
            children.Add(child);
        }

        return children;
    }

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
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }

    private static bool GetBool(object value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private void LogWarn(string message, Exception ex) => _logger.LogWarning(ex, message);

    private void LogError(string message, Exception ex) => _logger.LogError(ex, message);
}
