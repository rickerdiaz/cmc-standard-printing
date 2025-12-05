using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using CmcStandardPrinting.Domain.Categories;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(IConfiguration configuration, ILogger<CategoryController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codecategory:int}/{codetrans:int}")]
    public ActionResult<CategoryData> GetCategory(int codecategory, int codetrans)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_CategoryInfo";
            cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = codecategory;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cn.Close();

            var category = new CategoryData();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                category.Info = new Category
                {
                    Code = GetInt(row["Code"]),
                    Name = GetStr(row["Name"]),
                    Type = GetInt(row["Type"]),
                    Global = GetBool(row["Global"]),
                    Picture = GetStr(row["Picture"]),
                    Archive = GetInt(row["Archive"])
                };
                category.Profile = new User
                {
                    Code = GetInt(row["CodeUser"]),
                    CodeSite = GetInt(row["CodeSite"]),
                    UserName = GetStr(row["UserName"]),
                    Name = GetStr(row["NameUser"]),
                    Email = GetStr(row["Email"]),
                    RoleLevel = GetInt(row["RoleLevel"]),
                    SalesSite = GetInt(row["SalesSite"]),
                    SalesSiteLanguage = GetInt(row["SalesSiteLanguage"]),
                    SalesSiteRole = GetInt(row["SalesSiteRole"])
                };
            }

            if (ds.Tables.Count > 1)
            {
                foreach (DataRow tr in ds.Tables[1].Rows)
                {
                    category.Translation.Add(new CategoryTranslation
                    {
                        Name = GetStr(tr["Name"]),
                        Name2 = GetStr(tr["Name2"]),
                        CodeTrans = GetInt(tr["CodeTrans"]),
                        CodeSite = GetInt(tr["CodeSite"]),
                        NamePlural = GetStr(tr["NamePlural"])
                    });
                }
            }

            if (ds.Tables.Count > 2)
            {
                foreach (DataRow sh in ds.Tables[2].Rows)
                {
                    category.Sharing.Add(new SharingItem { Code = GetInt(sh["Code"]) });
                }
            }

            if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
            {
                var ar = ds.Tables[3].Rows[0];
                category.AutoNumber = new AutoNumber
                {
                    AutoNumberCodeSite = GetInt(ar["AutoNumberCodeSite"]),
                    AutoNumberFlag = GetBool(ar["AutoNumber"]),
                    AutoNumberPrefix = GetStr(ar["AutoNumberPrefix"]),
                    AutoNumberStart = GetStr(ar["AutoNumberStart"]),
                    AutoNumberKeepPrefixLength = GetBool(ar["AutoNumberKeepPrefixLength"])
                };
            }

            return Ok(category);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCategory failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveCategory([FromBody] CategoryData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing)
            {
                if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
            }
            var codeSiteList = "(" + string.Join(",", arrSharing.Cast<object>()) + ")";

            var isarchive = 0;
            var isactual = 0;
            if (data.Info.Archive == 1) { isactual = 1; isarchive = 0; }
            else if (data.Info.Archive == 2) { isactual = 0; isarchive = 1; }

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MANAGE_CATEGORYUPDATE";
            cmd.Parameters.Clear();
            var pCode = cmd.Parameters.Add("@Code", SqlDbType.Int);
            pCode.Value = data.Info.Code;
            pCode.Direction = ParameterDirection.InputOutput;
            cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
            cmd.Parameters.Add("@ListeType", SqlDbType.Int).Value = data.Info.Type;
            cmd.Parameters.Add("@CodeAcct", SqlDbType.NVarChar, 25).Value = string.Empty;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
            cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 2000).Value = codeSiteList;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
            cmd.Parameters.Add("@IsProduct", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 400).Value = data.Info.Picture;
            cmd.Parameters.Add("@IsActual", SqlDbType.Bit).Value = isactual;
            cmd.Parameters.Add("@IsArchive", SqlDbType.Bit).Value = isarchive;
            cmd.Parameters.Add("@CodesToMerge", SqlDbType.NVarChar, 2000).Value = string.Join(",", data.MergeList);
            var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new Exception($"[{resultCode}] Save category failed");

            var codeCategory = Convert.ToInt32(pCode.Value);
            if (codeCategory > 0)
            {
                cmd.CommandText = "sp_EgswItemTranslationUpdate";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var t in data.Translation)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codeCategory;
                    cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = t.Name;
                    cmd.Parameters.Add("@nvcName2", SqlDbType.NVarChar, 150).Value = t.Name2;
                    cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                    cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = t.CodeSite;
                    cmd.Parameters.Add("@tntListType", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                    cmd.Parameters.Add("@tntType", SqlDbType.Int).Value = data.Info.Type;
                    cmd.Parameters.Add("@nvcPlural", SqlDbType.NVarChar, 150).Value = t.NamePlural;
                    var retTr = cmd.Parameters.Add("@retval", SqlDbType.Int);
                    retTr.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(retTr.Value, -1);
                    if (resultCode != 0) throw new Exception($"[{resultCode}] Save category failed");
                }

                if (data.Info.Type == 8)
                {
                    cmd.CommandText = "sp_EgswUpdateAutoNumberDetailCategory";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.AutoNumber.AutoNumberCodeSite;
                    cmd.Parameters.Add("@intItemType", SqlDbType.Int, 260).Value = 8;
                    cmd.Parameters.Add("@blnAutoNumber", SqlDbType.Bit).Value = data.AutoNumber.AutoNumberFlag;
                    cmd.Parameters.Add("@vchPrefix", SqlDbType.NVarChar, 50).Value = data.AutoNumber.AutoNumberPrefix;
                    cmd.Parameters.Add("@vchStartingNum", SqlDbType.NVarChar, 50).Value = data.AutoNumber.AutoNumberStart;
                    cmd.Parameters.Add("@blnKeepLength", SqlDbType.Bit).Value = data.AutoNumber.AutoNumberKeepPrefixLength;
                    cmd.Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = codeCategory;
                    var retAuto = cmd.Parameters.Add("@retval", SqlDbType.Int);
                    retAuto.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(retAuto.Value, -1);
                    if (resultCode != 0) throw new Exception($"[{resultCode}] Save category failed");
                }
            }

            if (data.ActionType == 5 && data.MergeList.Count > 0)
            {
                var arrSites = new ArrayList();
                foreach (var s in data.MergeList) { if (!arrSites.Contains(s)) arrSites.Add(s); }
                var siteList = "(" + string.Join(",", arrSites.Cast<object>()) + ")";
                var sql = new System.Text.StringBuilder();

                sql.Clear();
                sql.Append("INSERT INTO EgswSharing ");
                sql.Append("SELECT @Code,0,CodeUserSharedTo,1,19,0,1,0 ");
                sql.Append("FROM EgswSharing ");
                sql.Append("WHERE Code=@Code AND CodeEgswTable=19 AND [Status]=1 AND [Type] IN (1,5) ");
                sql.Append("    AND CodeUserSharedTo NOT IN (");
                sql.Append("    SELECT DISTINCT CodeUserSharedTo ");
                sql.Append("    FROM EgswSharing  ");
                sql.Append("    WHERE Code IN " + siteList + " AND CodeEgswTable=19 AND [Status]=1 AND [Type] IN (1,5)");
                sql.Append(") ");
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codeCategory;
                cmd.ExecuteNonQuery();

                var mergeList = "(" + string.Join(",", data.MergeList) + ")";

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.CommandText = "UPDATE EgswListe SET  Category = @newCategoryCode WHERE Category IN " + mergeList;
                cmd.Parameters.Add("@newCategoryCode", SqlDbType.Int).Value = codeCategory;
                cmd.ExecuteNonQuery();

                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var code in arrSites.Cast<int>())
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = code;
                    cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWCATEGORY";
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                    cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = false;
                    var skip = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
                    skip.Direction = ParameterDirection.Output;
                    var retDel = cmd.Parameters.Add("@Return", SqlDbType.Int);
                    retDel.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(retDel.Value, -1);
                    if (resultCode != 0) throw new Exception($"[{resultCode}] Delete merged category failed");
                }
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = Convert.ToInt32(pCode.Value);
            response.Status = true;
            trans.Commit();
        }
        catch (Exception ex)
        {
            try { trans?.Rollback(); trans?.Dispose(); } catch { }
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Save category failed";
            _logger.LogError(ex, "SaveCategory failed");
            return StatusCode(500, response);
        }
        return Ok(response);
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{type:int}")]
    public ActionResult<List<GenericCodeValueList>> GetCategoryList(int codesite, int codetrans, int type)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWCATEGORY";
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var categories = new List<GenericCodeValueList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                categories.Add(new GenericCodeValueList
                {
                    Code = GetInt(r["Code"]),
                    Value = GetStr(r["Name"])
                });
            }
            return Ok(categories);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCategoryList failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{status:int}/{codeproperty:int?}/{name?}")]
    public ActionResult<List<Category>> SearchCategoryByName(int codesite, int codetrans, int type, int status, int codeproperty = -1, string? name = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(_configuration["dsn"] ?? ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWCATEGORY";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var categories = new List<Category>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                categories.Add(new Category
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    Global = GetBool(r["Global"])
                });
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var result = new List<Category>();
                foreach (var word in name.Trim().Split(','))
                {
                    var w = (word ?? string.Empty).Trim();
                    if (w.Length == 0) continue;
                    foreach (var c in categories)
                    {
                        if ((c.Name ?? string.Empty).ToLowerInvariant().Contains(w.ToLowerInvariant()))
                            result.Add(c);
                    }
                }
                categories = result;
            }

            return Ok(categories);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchCategoryByName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("search")]
    public ActionResult<List<Category>> SearchCategoryByName2([FromBody] ConfigurationcSearch data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(_configuration["dsn"] ?? ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = data.Type;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = data.Status;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWCATEGORY";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var categories = new List<Category>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                categories.Add(new Category
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    Global = GetBool(r["Global"])
                });
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var result = new List<Category>();
                foreach (var word in data.Name.Trim().Split(','))
                {
                    var w = (word ?? string.Empty).Trim();
                    if (w.Length == 0) continue;
                    foreach (var c in categories)
                    {
                        if ((c.Name ?? string.Empty).ToLowerInvariant().Contains(w.ToLowerInvariant()))
                            result.Add(c);
                    }
                }
                categories = result;
            }

            return Ok(categories);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchCategoryByName2 failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("autonumber/{codecategory:int}/{codesite:int}")]
    public ActionResult<AutoNumber> GetCategoryAutonumber(int codecategory, int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_EgswGetAutoNumberDetailCategory]";
            cmd.Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = codecategory;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var autoNumber = new AutoNumber();
            if (ds.Tables[0].Rows.Count < 1) return Ok(autoNumber);
            var row1 = ds.Tables[0].Rows[0];
            autoNumber.AutoNumberPrefix = GetStr(row1["Prefix"]);
            autoNumber.AutoNumberFlag = GetBool(row1["AutoNumber"]);
            autoNumber.AutoNumberCodeSite = GetInt(row1["CodeSite"]);
            autoNumber.AutoNumberKeepPrefixLength = GetBool(row1["KeepLength"]);
            autoNumber.AutoNumberStart = GetStr(row1["StartingNumber"]);
            return Ok(autoNumber);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCategoryAutonumber failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("translation/{codecategory:int}/{codesite:int}")]
    public ActionResult<List<RecipeTranslation>> GetCategoryTranslation(int codecategory, int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_CategoryTranslation]";
            cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = codecategory;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var translations = new List<RecipeTranslation>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var isArchive = Convert.ToBoolean(r["isArchive"]);
                var isActual = Convert.ToBoolean(r["isActual"]);
                var archive = isArchive ? 2 : isActual ? 1 : 0;
                translations.Add(new RecipeTranslation
                {
                    CodeTrans = GetInt(r["CodeTrans"]),
                    TranslationName = GetStr(r["TranslationName"]),
                    Name = GetStr(r["Name"]),
                    Picture = GetStr(r["Picture"]),
                    HasPicture = 1,
                    Archive = archive
                });
            }
            return Ok(translations);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCategoryTranslation failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeleteCategory([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            var arrCategoryCodes = new ArrayList();
            foreach (var c in data.CodeList)
            {
                if (!arrCategoryCodes.Contains(c.Code)) arrCategoryCodes.Add(c.Code);
            }
            var codeCategoryList = string.Join(",", arrCategoryCodes.Cast<object>().Select(x => Convert.ToString(x)));

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeCategoryList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWCATEGORY";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
            var skipList = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
            skipList.Direction = ParameterDirection.Output;
            var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new Exception($"[{resultCode}] Delete category failed");

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(skipList.Value);
            response.Status = true;
        }
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Delete category failed";
            _logger.LogError(ex, "DeleteCategory failed");
            return StatusCode(500, response);
        }
        return Ok(response);
    }

    [HttpPost("purge/{type:int}/{codeUser:int}/{codeSite:int}")]
    public ActionResult<ResponseCallBack> PurgeCategory(int type, long codeUser, long codeSite)
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
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeUser;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWCATEGORY";
            var skipList = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
            skipList.Direction = ParameterDirection.Output;
            var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();

            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new Exception($"[{resultCode}] Purge category failed");
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(skipList.Value);
            response.Status = true;
            trans.Commit();
        }
        catch (Exception ex)
        {
            try { trans?.Rollback(); trans?.Dispose(); } catch { }
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Purge category failed";
            _logger.LogError(ex, "PurgeCategory failed");
            return StatusCode(500, response);
        }
        return Ok(response);
    }

    [HttpPost("upload")]
    public ActionResult UploadPicture()
    {
        try
        {
            if (Request.Form?.Files == null || Request.Form.Files.Count == 0) return Problem(title: "No file found", statusCode: 400);
            var file = Request.Form.Files[0];
            var folder = TempFolder2;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var dest = Path.Combine(folder, file.FileName);
            using (var fs = new FileStream(dest, FileMode.Create))
            {
                file.CopyTo(fs);
            }
            return Ok(file.FileName);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadPicture failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("uploads")]
    public ActionResult UploadPicture2()
    {
        return UploadPicture();
    }

    [HttpGet("img/{imgname}")]
    public ActionResult<byte[]> GetImage(string imgname)
    {
        try
        {
            var dest = Path.Combine(PicOriginalFolder, imgname);
            var data = System.IO.File.ReadAllBytes(dest);
            return File(data, "image/jpeg");
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetImage failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("saveimage/{codecategory:int}/{type:int}")]
    public ActionResult SaveImage(int codecategory, int type)
    {
        int resultCode = 0;
        try
        {
            var tempName = GetStr(Request.Form["TempName"]);
            var codeSite = Convert.ToInt32(Request.Form["codeSite"]);
            var codeUser = Convert.ToInt32(Request.Form["codeUser"]);
            var pic = GetStr(Request.Form["PictureName"]);
            var archive = Convert.ToInt32(Request.Form["Archive"]);

            var source = Path.Combine(TempFolder2, tempName);
            var dest = Path.Combine(DamFolder, pic);
            var dest2 = Path.Combine(PicNormalFolder, pic);
            var dest3 = Path.Combine(PicThumbnailFolder, pic);

            if (!System.IO.File.Exists(dest))
                System.IO.File.Copy(source, dest);
            if (!System.IO.File.Exists(dest2))
                ResizeConvertImage(source, dest2, 800, 600);
            if (!System.IO.File.Exists(dest3))
                ResizeConvertImage(source, dest3, 150, 150, true);

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_Manage_Category_UpdatePicture";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codecategory;
            cmd.Parameters.Add("@Archive", SqlDbType.Int).Value = archive;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeUser;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 400).Value = pic;
            var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new Exception($"[{resultCode}] Save category image failed");
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            _logger.LogError(ex, "SaveImage failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
        return Ok(resultCode);
    }

    [HttpPost("deleteimage/{codecategory:int}/{type:int}")]
    public ActionResult DeleteImage(int codecategory, int type, [FromBody] CategoryTranslation data)
    {
        int resultCode = 0;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_Manage_Category_UpdatePicture";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codecategory;
            cmd.Parameters.Add("@Archive", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 400).Value = string.Empty;
            var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new Exception($"[{resultCode}] Delete category image failed");
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            _logger.LogError(ex, "DeleteImage failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
        return Ok(resultCode);
    }

    private string TempFolder2 => (GetStr(_configuration["temp"]) is { Length: > 0 } tmp ? tmp : MapPath("temp")).TrimEnd('\\') + "\\";
    private string DamFolder => (GetStr(_configuration["dam"]) is { Length: > 0 } tmp ? tmp : MapPath("DigitalAssets")).TrimEnd('\\') + "\\";
    private string PicNormalFolder => (GetStr(_configuration["picnormal"]) is { Length: > 0 } tmp ? tmp : MapPath("picnormal")).TrimEnd('\\') + "\\";
    private string PicThumbnailFolder => (GetStr(_configuration["picthumbnail"]) is { Length: > 0 } tmp ? tmp : MapPath("picthumbnail")).TrimEnd('\\') + "\\";
    private string PicOriginalFolder => (GetStr(_configuration["picoriginal"]) is { Length: > 0 } tmp ? tmp : MapPath("picoriginal")).TrimEnd('\\') + "\\";

    private string MapPath(string relative) => Path.Combine(AppContext.BaseDirectory, relative);

    private static bool ResizeConvertImage(string strFile, string strDestination, int newWidth, int newHeight, bool delete = false)
    {
        try
        {
            using var originalBitmap = (Bitmap)Image.FromFile(strFile, true);
            var ratio = (double)originalBitmap.Height / originalBitmap.Width;
            double tempW, tempH;
            if (newHeight > newWidth) { tempH = newHeight; tempW = tempH / ratio; }
            else { tempW = newWidth; tempH = tempW * ratio; }
            while (tempW > newWidth || tempH > newHeight) { tempW *= 0.999; tempH *= 0.999; }
            var W1 = (int)tempW; var H1 = (int)tempH;
            using var newbmp = new Bitmap(W1, H1);
            using (var g = Graphics.FromImage(newbmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.Clear(Color.White);
                g.DrawImage(originalBitmap, 0, 0, W1, H1);
            }
            newbmp.Save(strDestination, System.Drawing.Imaging.ImageFormat.Jpeg);
            if (delete) System.IO.File.Delete(strFile);
            return true;
        }
        catch
        {
            return false;
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

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (bool.TryParse(Convert.ToString(value), out var b)) return b;
        try { return Convert.ToInt32(value) != 0; } catch { return false; }
    }
}
