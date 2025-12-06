using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PublicationController> _logger;

    public PublicationController(IConfiguration configuration, ILogger<PublicationController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codesite:int}/{codeproperty:int?}/{name?}")]
    public ActionResult<List<TreeNode>> GetPublication(int codesite, int codeproperty = -1, string name = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Placement]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var placements = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                placements.Add(new GenericTree
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    ParentCode = GetInt(r["ParentCode"]),
                    ParentName = GetStr(r["ParentName"]),
                    Flagged = GetBool(r["IsParent"])
                });
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var placementschildren = new List<GenericTree>();
                var placementsresult = new List<GenericTree>();
                foreach (var word in name.Split(','))
                {
                    var w = ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                    foreach (var k in placements)
                    {
                        if (k.Name.ToLowerInvariant().Contains(w) && !placementschildren.Contains(k))
                            placementschildren.Add(k);
                    }
                }
                foreach (var kid in placementschildren)
                {
                    placementsresult.Add(kid);
                    var currentParentCode = kid.ParentCode;
                    while (currentParentCode > 0)
                    {
                        var parent = GetParent(currentParentCode, placements);
                        if (!placementsresult.Contains(parent)) placementsresult.Add(parent);
                        currentParentCode = parent.ParentCode;
                    }
                }
                placements = placementsresult;
            }

            var placementdata = new List<TreeNode>();
            var parents = placements.Where(o => o.ParentCode == 0 || o.ParentCode == -99).OrderBy(o => o.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new TreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = CreateChildren(placements, p.Code),
                    Select = p.Flagged,
                    ParentTitle = p.ParentName
                };
                placementdata.Add(parent);
            }
            return Ok(placementdata);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetPublication: bad request");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPublication: database error");
            return StatusCode(500);
        }
    }

    [HttpPost("search")]
    public ActionResult<List<TreeNode>> GetPublication2([FromBody] ConfigurationcSearch data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Placement]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var placements = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                placements.Add(new GenericTree
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    ParentCode = GetInt(r["ParentCode"]),
                    ParentName = GetStr(r["ParentName"]),
                    Flagged = GetBool(r["IsParent"])
                });
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var placementschildren = new List<GenericTree>();
                var placementsresult = new List<GenericTree>();
                foreach (var word in data.Name.Split(','))
                {
                    var w = ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                    foreach (var k in placements)
                    {
                        if (k.Name.ToLowerInvariant().Contains(w) && !placementschildren.Contains(k))
                            placementschildren.Add(k);
                    }
                }
                foreach (var kid in placementschildren)
                {
                    placementsresult.Add(kid);
                    var currentParentCode = kid.ParentCode;
                    while (currentParentCode > 0)
                    {
                        var parent = GetParent(currentParentCode, placements);
                        if (!placementsresult.Contains(parent)) placementsresult.Add(parent);
                        currentParentCode = parent.ParentCode;
                    }
                }
                placements = placementsresult;
            }

            var placementdata = new List<TreeNode>();
            var parents = placements.Where(o => o.ParentCode == 0 || o.ParentCode == -99 || o.Flagged).OrderBy(o => o.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new TreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = CreateChildren(placements, p.Code),
                    Select = p.Flagged,
                    ParentTitle = p.ParentName
                };
                if (p.ParentCode == 0)
                    placementdata.Add(parent);
            }
            return Ok(placementdata);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetPublication2: bad request");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPublication2: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/publicationlist/{codesite:int}/{codeproperty:int?}")]
    public ActionResult<List<GenericList>> GetPublicationList(int codesite, int codeproperty = -1)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Placement]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var placements = new List<GenericList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                placements.Add(new GenericList
                {
                    Code = GetInt(r["Code"]),
                    Value = GetStr(r["Name"]),
                    IsParent = GetBool(r["IsParent"])
                });
            }
            return Ok(placements);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetPublicationList: bad request");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPublicationList: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("sharing/{codeplacement:int}")]
    public ActionResult<List<TreeNode>> GetPublicationSharing(int codeplacement)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_SharingPlacement]";
            cmd.Parameters.Add("@CodePlacement", SqlDbType.Int).Value = codeplacement;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var sharings = new List<GenericTree>();
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

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (sharingdata.All(obj => obj.Key != p.Code))
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
                    if (parent.Children is { Count: > 0 })
                        sharingdata.Add(parent);
                }
            }
            return Ok(sharingdata);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetPublicationSharing: bad request");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPublicationSharing: database error");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SavePublication([FromBody] GenericData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing ?? new List<GenericList>())
            {
                if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
            }
            var codeSiteList = Join(arrSharing, string.Empty, string.Empty, ",");
            if (string.IsNullOrWhiteSpace(codeSiteList) && data.Info.Global)
                codeSiteList = data.Info.CodeSite.ToString(CultureInfo.InvariantCulture);

            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[API_Manage_Publication_Update]";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
            cmd.Parameters.Add("@CodeParent", SqlDbType.Int).Value = data.Info.ParentCode;
            cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 4000).Value = codeSiteList;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
            cmd.Parameters.Add("@IsParent", SqlDbType.Bit).Value = data.Info.IsParent;
            cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            cmd.Parameters["@Code"].Direction = ParameterDirection.InputOutput;
            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
            if (resultCode != 0)
            {
                trans.Rollback();
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save publication failed";
                response.ReturnValue = string.Empty;
                return StatusCode(500, response);
            }
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetInt(cmd.Parameters["@Code"].Value, -1);
            response.Status = true;
            trans.Commit();
            return Ok(response);
        }
        catch (Exception ex)
        {
            try { trans?.Rollback(); } catch { }
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Save publication failed";
            _logger.LogError(ex, "SavePublication: database error");
            return StatusCode(500, response);
        }
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeletePublication([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            var arrPublicationCodes = new ArrayList();
            foreach (var c in data.CodeList ?? new List<DeleteCode>())
            {
                if (!arrPublicationCodes.Contains(c.Code)) arrPublicationCodes.Add(c.Code);
            }
            var codePublicationList = Join(arrPublicationCodes, string.Empty, string.Empty, ",");
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codePublicationList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "PLACEMENT";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
            cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
            if (resultCode != 0)
            {
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Delete publication failed";
                response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                return StatusCode(500, response);
            }
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Unexpected error occured";
            _logger.LogError(ex, "DeletePublication: database error");
            return StatusCode(500, response);
        }
    }

    private static GenericTree GetParent(int parentcode, List<GenericTree> keywords)
        => keywords.Single(obj => obj.Code == parentcode);

    private static List<TreeNode> CreateChildren(List<GenericTree> keyworddata, int code)
    {
        var children = new List<TreeNode>();
        if (keyworddata != null)
        {
            var kids = keyworddata.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new TreeNode
                {
                    Title = k.Name,
                    ParentTitle = k.ParentName,
                    Key = k.Code,
                    Icon = false,
                    Children = CreateChildren(keyworddata, k.Code),
                    Select = k.Flagged
                };
                children.Add(child);
            }
        }
        return children;
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> sharingdata, int code)
    {
        var children = new List<TreeNode>();
        if (sharingdata != null)
        {
            var kids = sharingdata.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new TreeNode
                {
                    Title = k.Name,
                    Key = k.Code,
                    Icon = false,
                    Children = new List<TreeNode>(),
                    Select = k.Flagged,
                    ParentTitle = k.ParentName,
                    Note = k.Global
                };
                children.Add(child);
            }
        }
        return children;
    }

    private static string Join(ArrayList list, string prefix, string suffix, string separator)
    {
        var val = string.Empty;
        foreach (var c in list)
        {
            if (!string.IsNullOrEmpty(val)) val += separator;
            val += c?.ToString();
        }
        return string.IsNullOrEmpty(val) ? string.Empty : prefix + val + suffix;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
        try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
    }

    private static string GetStr(object? value)
        => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var result = value;
        var specialChars = new Dictionary<string, string>
        {
            { "ä", "ae" },
            { "ö", "oe" },
            { "ü", "ue" },
            { "ß", "ss" },
            { "é", "e" },
            { "è", "e" },
            { "ê", "e" },
            { "à", "a" },
            { "á", "a" },
            { "â", "a" },
            { "ù", "u" },
            { "û", "u" },
            { "ú", "u" },
            { "î", "i" },
            { "ï", "i" }
        };

        foreach (var k in specialChars.Keys)
        {
            result = result.Replace(k, specialChars[k], StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}
