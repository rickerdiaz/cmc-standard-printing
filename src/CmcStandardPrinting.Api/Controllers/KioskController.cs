using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Globalization;
using System.Linq;
using CmcStandardPrinting.Domain.Kiosks;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KioskController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KioskController> _logger;

    public KioskController(IConfiguration configuration, ILogger<KioskController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codeliste:int}/{showall:bool}/{codesite:int}")]
    public ActionResult<List<RecipeBrandSite>> GetKiosk(int codeliste, bool showall, int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[GET_RecipeBrandSiteCM]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = codeliste;
            cmd.Parameters.Add("@ShowAll", SqlDbType.Bit, 1).Value = showall;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int, 4).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var brandSites = new List<RecipeBrandSite>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                brandSites.Add(new RecipeBrandSite
                {
                    Code = GetInt(r["CodeBrandSite"]),
                    Name = GetStr(r["BrandSite"]),
                    Enabled = GetBool(r["IsSelected"]),
                    DateFrom = GetDateString(r["BrandSiteDateFrom"]),
                    DateTo = GetDateString(r["BrandSiteDateTo"])
                });
            }

            return Ok(brandSites.ToList());
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetKiosk: invalid parameters");
            return Problem(title: "Invalid request", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKiosk failed for {CodeListe}/{CodeSite}", codeliste, codesite);
            return StatusCode(500);
        }
    }

    [HttpGet("{codesite:int}/{type:int}/{tree:bool?}/{name?}")]
    public ActionResult<object> GetKioskByName(int codesite, int type, bool tree = false, string name = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "BRANDSITE";
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (tree)
            {
                var kiosk = new List<GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    kiosk.Add(new GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = 0,
                        ParentName = string.Empty,
                        Flagged = false,
                        Type = 0,
                        Global = GetBool(r["Global"])
                    });
                }

                var kiosklist = new List<TreeNode>();
                var parents = kiosk.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (kiosklist.All(obj => obj.Key != p.Code))
                    {
                        var parent = new TreeNode
                        {
                            Title = p.Name,
                            Key = p.Code,
                            Icon = false,
                            Children = CreateChildren(kiosk, p.Code),
                            Select = p.Flagged,
                            ParentTitle = p.ParentName
                        };
                        kiosklist.Add(parent);
                    }
                }
                return Ok(kiosklist.ToList());
            }

            var kioskEntries = new List<Kiosk>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                kioskEntries.Add(new Kiosk
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    Global = GetBool(r["Global"])
                });
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var kioskresult = new List<Kiosk>();
                var arrNames = name.Trim().Split(',');
                foreach (var word in arrNames)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        var key = ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var s in kioskEntries)
                        {
                            if (s.Name.ToLowerInvariant().Contains(key))
                            {
                                kioskresult.Add(s);
                            }
                        }
                    }
                }

                kioskEntries = kioskresult;
            }

            return Ok(kioskEntries.ToList());
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetKioskByName: invalid parameters");
            return Problem(title: "Invalid request", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKioskByName failed for {CodeSite}/{Type}", codesite, type);
            return StatusCode(500);
        }
    }

    [HttpPost("search")]
    public ActionResult<object> GetKioskByName2([FromBody] KioskSearch data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "BRANDSITE";
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var kiosk = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                kiosk.Add(new GenericTree
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    ParentCode = 0,
                    ParentName = string.Empty,
                    Flagged = false,
                    Type = 0,
                    Global = GetBool(r["Global"])
                });
            }

            if (string.IsNullOrEmpty(data.Name))
            {
                var kiosklist = new List<TreeNode>();
                var parents = kiosk.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (kiosklist.All(obj => obj.Title != p.Name))
                    {
                        var parent = new TreeNode
                        {
                            Title = p.Name,
                            Key = p.Code,
                            Icon = false,
                            Children = CreateChildren(kiosk, p.Code),
                            Select = p.Flagged,
                            ParentTitle = p.ParentName,
                            Note = p.Global
                        };
                        kiosklist.Add(parent);
                    }
                }
                return Ok(kiosklist.ToList());
            }

            var kiosklistWithFilter = new List<TreeNode>();
            var parentsFiltered = kiosk.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parentsFiltered)
            {
                if (kiosklistWithFilter.All(obj => obj.Title != p.Name))
                {
                    var parent = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildren(kiosk, p.Code),
                        Select = p.Flagged,
                        ParentTitle = p.ParentName,
                        Note = p.Global
                    };
                    kiosklistWithFilter.Add(parent);
                }
            }

            var kioskresult = new List<TreeNode>();
            var arrNames = data.Name.Trim().Split(',');
            foreach (var word in arrNames)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    var key = ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                    foreach (var s in kiosklistWithFilter)
                    {
                        if (s.Title.ToLowerInvariant().Contains(key))
                        {
                            kioskresult.Add(s);
                        }
                    }
                }
            }

            return Ok(kioskresult.ToList());
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetKioskByName2: invalid parameters");
            return Problem(title: "Invalid request", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKioskByName2 failed for {CodeSite}", data.CodeSite);
            return StatusCode(500);
        }
    }

    [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codekiosk:int}")]
    public ActionResult<List<TreeNode>> GetKioskSharing(int codesite, int type, int tree, int codekiosk)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_SharingAll]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codekiosk;
            cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 151;
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
                        Children = CreateChildren(sharings, p.Code),
                        Select = p.Flagged,
                        ParentTitle = p.ParentName
                    };
                    sharingdata.Add(parent);
                }
            }
            return Ok(sharingdata);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetKioskSharing: invalid parameters");
            return Problem(title: "Invalid request", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKioskSharing failed for {CodeSite}/{CodeKiosk}", codesite, codekiosk);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/getkiosk/{codesite:int}/{codekiosk:int?}")]
    public ActionResult<List<Kiosk>> GetKioskList(int codesite, int codekiosk = -1)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_KioskCodeName]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@CodeKiosk", SqlDbType.Int).Value = codekiosk;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var kiosk = new List<Kiosk>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                kiosk.Add(new Kiosk
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    Global = GetBool(r["Global"])
                });
            }
            return Ok(kiosk.ToList());
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetKioskList: invalid parameters");
            return Problem(title: "Invalid request", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKioskList failed for {CodeSite}/{CodeKiosk}", codesite, codekiosk);
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveKiosk([FromBody] KioskData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            using var cmd = new SqlCommand();
            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing)
            {
                if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
            }

            var codeSiteList = Join(arrSharing, string.Empty, string.Empty, ",");

            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[INSERT_BrandSite]";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@retval", SqlDbType.Int);
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
            cmd.Parameters.Add("@intId", SqlDbType.Int).Value = data.Info.Code;
            cmd.Parameters.Add("@vrName", SqlDbType.NVarChar, 50).Value = data.Info.Name;
            cmd.Parameters.Add("@vrSearch", SqlDbType.NVarChar, 50).Value = data.Info.Name;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
            cmd.Parameters.Add("@intMetImpBoth", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@strCodeSiteList", SqlDbType.NVarChar, 8000).Value = codeSiteList;
            cmd.Parameters["@intId"].Direction = ParameterDirection.InputOutput;
            cmd.Parameters["@retval"].Direction = ParameterDirection.ReturnValue;
            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            var codeKiosk = GetInt(cmd.Parameters["@intId"].Value, -1);
            resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
            if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Save kiosk failed"));

            if (codeKiosk != -1)
            {
                cmd.Connection = cn;
                cmd.CommandText = $"DELETE FROM EgswSharing WHERE Code={codeKiosk} AND CodeUserOwner={data.Profile.CodeSite} AND CodeEgswTable=151 AND Type=1";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.ExecuteNonQuery();

                cmd.CommandText = "sp_EgswUpdateSharing";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@intCode", SqlDbType.Int);
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int);
                cmd.Parameters.Add("@intCodeSitesShared", SqlDbType.Int);
                cmd.Parameters.Add("@intCodeEgswTable", SqlDbType.Int);
                cmd.Parameters.Add("@isGlobal", SqlDbType.Bit);

                if (!string.IsNullOrEmpty(codeSiteList) && !codeSiteList.Equals("-1", StringComparison.Ordinal))
                {
                    var cleanedList = codeSiteList.Replace("(", string.Empty, StringComparison.Ordinal).Replace(")", string.Empty, StringComparison.Ordinal);
                    var arrCodeSites = cleanedList.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var siteStr in arrCodeSites)
                    {
                        if (int.TryParse(siteStr, out var site))
                        {
                            cmd.Parameters["@intCode"].Value = codeKiosk;
                            cmd.Parameters["@intCodeSite"].Value = data.Profile.CodeSite;
                            cmd.Parameters["@intCodeSitesShared"].Value = site;
                            cmd.Parameters["@intCodeEgswTable"].Value = 151;
                            cmd.Parameters["@isGlobal"].Value = data.Info.Global;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else if (codeSiteList.Equals("-1", StringComparison.Ordinal))
                {
                    var cleanedList = codeSiteList.Replace("(", string.Empty, StringComparison.Ordinal).Replace(")", string.Empty, StringComparison.Ordinal);
                    cmd.Parameters["@intCode"].Value = codeKiosk;
                    cmd.Parameters["@intCodeSite"].Value = data.Profile.CodeSite;
                    cmd.Parameters["@intCodeSitesShared"].Value = int.Parse(cleanedList, CultureInfo.InvariantCulture);
                    cmd.Parameters["@intCodeEgswTable"].Value = 151;
                    cmd.Parameters["@isGlobal"].Value = data.Info.Global;
                    cmd.ExecuteNonQuery();
                }

                if (data.Keywords.Count == 0)
                {
                    cmd.CommandText = $"DELETE FROM EgswBrandSiteKeywords WHERE CodeBrandSite={codeKiosk}";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = $"DELETE FROM EgswBrandSiteKeywords WHERE CodeBrandSite={codeKiosk}";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();
                    foreach (var keyword in data.Keywords)
                    {
                        cmd.CommandText = "UPDATE_KeywordsBrandSite";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeKeyword", SqlDbType.Int, 4).Value = keyword.Code;
                        cmd.Parameters.Add("@CodeBrandSite", SqlDbType.Int, 4).Value = codeKiosk;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            response.Code = 0; response.Message = "OK"; response.ReturnValue = codeKiosk; response.Status = true;
            trans.Commit();
            return Ok(response);
        }
        catch (Exception ex)
        {
            try { trans?.Rollback(); } catch { }
            if (resultCode == 0) resultCode = 500;
            _logger.LogError(ex, "SaveKiosk failed for {User}/{Code}", data?.Profile?.Code, data?.Info?.Code);
            return StatusCode(500, Fail(response, resultCode, "Save kiosk failed"));
        }
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeleteKiosk([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlParameter? skipListParam = null;
        try
        {
            using var cmd = new SqlCommand();
            var arrCategoryCodes = new ArrayList();
            foreach (var c in data.CodeList)
            {
                if (!arrCategoryCodes.Contains(c.Code)) arrCategoryCodes.Add(c.Code);
            }
            var codeKioskList = Join(arrCategoryCodes, string.Empty, string.Empty, ",");
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeKioskList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "BRANDSITE";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
            skipListParam = cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000);
            skipListParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
            if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Delete kiosk failed"));
            response.Code = 0; response.Message = "OK"; response.ReturnValue = GetStr(skipListParam.Value); response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode; response.ReturnValue = GetStr(skipListParam?.Value); response.Status = false; response.Message = "Delete kiosk failed";
            _logger.LogError(ex, "DeleteKiosk failed for {Codes}", string.Join(',', data.CodeList.Select(c => c.Code)));
            return StatusCode(500, response);
        }
    }

    [HttpGet("keywords/{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codekiosk:int}")]
    public ActionResult<List<TreeNode>> GetKioskKeywords(int codesite, int codetrans, int type, int tree, int codekiosk)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[GET_KeywordsbySharing]";
            cmd.Parameters.Add("@ListeType", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = tree;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 151;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = 0;
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
                    ParentName = GetStr(r["ParentName"])
                });
            }

            var keywordsdata = new List<TreeNode>();
            var parents = keywords.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (keywordsdata.All(obj => obj.Key != p.Code))
                {
                    var parent = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateKeywordChildren(keywords, p.Code, codekiosk),
                        Select = CheckIfSelected(codekiosk, p.Code),
                        ParentTitle = p.ParentName
                    };
                    keywordsdata.Add(parent);
                }
            }
            return Ok(keywordsdata);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetKioskKeywords: invalid parameters");
            return Problem(title: "Invalid request", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetKioskKeywords failed for {CodeKiosk}", codekiosk);
            return StatusCode(500);
        }
    }

    private static List<TreeNode> CreateChildren(List<GenericTree> sharingdata, int code)
    {
        var children = new List<TreeNode>();
        var kids = sharingdata.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = null,
                Select = k.Flagged,
                ParentTitle = k.ParentName,
                Note = k.Global
            };
            children.Add(child);
        }
        return children;
    }

    private List<TreeNode> CreateKeywordChildren(List<GenericTree> keywordsdata, int code, int codekiosk)
    {
        var children = new List<TreeNode>();
        var kids = keywordsdata.Where(obj => obj.ParentCode == code).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = null,
                Select = CheckIfSelected(codekiosk, k.Code),
                ParentTitle = k.ParentName,
                Note = k.Global
            };
            children.Add(child);
        }
        return children;
    }

    private bool CheckIfSelected(int codekiosk, int codekeyword)
    {
        const string query = "SELECT CASE WHEN (COUNT(*)>0) THEN 1 ELSE 0 END FROM EgswBrandSiteKeywords WHERE CodeBrandsite = @codekiosk AND CodeKeyword = @codekeyword";
        try
        {
            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(query, cn);
            cmd.Parameters.Add("@codekiosk", SqlDbType.Int).Value = codekiosk;
            cmd.Parameters.Add("@codekeyword", SqlDbType.Int).Value = codekeyword;
            cn.Open();
            var result = cmd.ExecuteScalar();
            return GetBool(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CheckIfSelected fallback to false for {CodeKiosk}/{CodeKeyword}", codekiosk, codekeyword);
            return false;
        }
    }

    private static int GetInt(object value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i;
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
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private static string GetDateString(object value)
    {
        if (value == null || value == DBNull.Value) return string.Empty;
        if (value is DateTime dt) return dt.ToString(CultureInfo.InvariantCulture);
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static string Join(ICollection arr, string prefix, string suffix, string delimiter, string valuePrefixAndSuffix = "")
    {
        if (arr.Count == 0) return string.Empty;
        var parts = new List<string>();
        foreach (var item in arr)
        {
            parts.Add($"{valuePrefixAndSuffix}{item}{valuePrefixAndSuffix}");
        }

        return string.Concat(prefix, string.Join(delimiter, parts), suffix);
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        return value.Replace("ä", "ae", StringComparison.OrdinalIgnoreCase)
            .Replace("ö", "oe", StringComparison.OrdinalIgnoreCase)
            .Replace("ü", "ue", StringComparison.OrdinalIgnoreCase)
            .Replace("ß", "ss", StringComparison.OrdinalIgnoreCase)
            .Replace("ç", "c", StringComparison.OrdinalIgnoreCase)
            .Replace("à", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("á", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("â", "a", StringComparison.OrdinalIgnoreCase)
            .Replace("è", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("é", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("ê", "e", StringComparison.OrdinalIgnoreCase)
            .Replace("î", "i", StringComparison.OrdinalIgnoreCase)
            .Replace("ï", "i", StringComparison.OrdinalIgnoreCase)
            .Replace("ô", "o", StringComparison.OrdinalIgnoreCase)
            .Replace("ù", "u", StringComparison.OrdinalIgnoreCase)
            .Replace("û", "u", StringComparison.OrdinalIgnoreCase);
    }

    private static ResponseCallBack Fail(ResponseCallBack r, int code, string message)
    {
        r.Code = code;
        r.Message = message;
        r.Status = false;
        return r;
    }
}
