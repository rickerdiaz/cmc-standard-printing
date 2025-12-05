using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codecategory:int}/{codetrans:int}")]
        public ActionResult<Models.CategoryData> GetCategory(int codecategory, int codetrans)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Common.SP_API_GET_CategoryInfo;
                cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = codecategory;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cn.Close();
                var categories = new Models.CategoryData();
                // TODO: map ds -> categories
                return Ok(categories);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SaveCategory([FromBody] Models.CategoryData data)
        {
            var response = new Models.ResponseCallBack();
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
                cmd.Parameters.Add("@CodeAcct", SqlDbType.NVarChar, 25).Value = "";
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
                        cmd.Parameters.Add("@blnAutoNumber", SqlDbType.Bit).Value = data.AutoNumber.AutoNumber;
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
                    var sql = new StringBuilder();

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
            catch (Exception)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save category failed";
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        [HttpGet("{codesite:int}/{codetrans:int}/{type:int}")]
        public ActionResult<List<Models.GenericCodeValueList>> GetCategoryList(int codesite, int codetrans, int type)
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

                var categories = new List<Models.GenericCodeValueList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    categories.Add(new Models.GenericCodeValueList
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
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{status:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.Category>> SearchCategoryByName(int codesite, int codetrans, int type, int status, int codeproperty = -1, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(HttpContext.RequestServices.GetService<IConfiguration>()?["dsn"] ?? ConnectionString);
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

                var categories = new List<Models.Category>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    categories.Add(new Models.Category
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var result = new List<Models.Category>();
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
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost("search")]
        public ActionResult<List<Models.Category>> SearchCategoryByName2([FromBody] Models.ConfigurationcSearch data)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(HttpContext.RequestServices.GetService<IConfiguration>()?["dsn"] ?? ConnectionString);
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

                var categories = new List<Models.Category>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    categories.Add(new Models.Category
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var result = new List<Models.Category>();
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
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("autonumber/{codecategory:int}/{codesite:int}")]
        public ActionResult<Models.AutoNumber> GetCategoryAutonumber(int codecategory, int codesite)
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

                var autoNumber = new Models.AutoNumber();
                if (ds.Tables[0].Rows.Count < 1) return Ok(autoNumber);
                var row1 = ds.Tables[0].Rows[0];
                autoNumber.AutoNumberPrefix = GetStr(row1["Prefix"]);
                autoNumber.AutoNumber = GetBool(row1["AutoNumber"]);
                autoNumber.AutoNumberCodeSite = GetInt(row1["CodeSite"]);
                autoNumber.AutoNumberKeepPrefixLength = GetBool(row1["KeepLength"]);
                autoNumber.AutoNumberStart = GetStr(row1["StartingNumber"]);
                return Ok(autoNumber);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("translation/{codecategory:int}/{codesite:int}")]
        public ActionResult<List<Models.RecipeTranslation>> GetCategoryTranslation(int codecategory, int codesite)
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

                var translations = new List<Models.RecipeTranslation>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var isArchive = Convert.ToBoolean(r["isArchive"]);
                    var isActual = Convert.ToBoolean(r["isActual"]);
                    var archive = isArchive ? 2 : isActual ? 1 : 0;
                    translations.Add(new Models.RecipeTranslation
                    {
                        CodeTrans = GetInt(r["CodeTrans"]),
                        TranslationName = GetStr(r["TranslationName"]),
                        Name = GetStr(r["Name"]),
                        Picture = GetStr(r["Picture"]),
                        hasPicture = 1,
                        Archive = archive
                    });
                }
                return Ok(translations);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteCategory([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
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
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.ReturnValue = GetStr("");
                response.Message = "Delete category failed";
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        [HttpPost("purge/{type:int}/{codeUser:int}/{codeSite:int}")]
        public ActionResult<Models.ResponseCallBack> PurgeCategory(int type, long codeUser, long codeSite)
        {
            var response = new Models.ResponseCallBack();
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
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWCATEGORY";
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0)
                {
                    if (resultCode == -480)
                    {
                        response.Code = -480;
                        response.Message = "Nothing was deleted";
                        response.Status = false;
                    }
                    else
                    {
                        throw new Exception($"[{resultCode}] Purge category failed");
                    }
                }
                else
                {
                    response.Code = 0;
                    response.Message = "OK";
                    response.Status = true;
                }
                trans.Commit();
            }
            catch (Exception)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Purge category failed";
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> sharings, int code)
        {
            var children = new List<Models.TreeNode>();
            var kids = sharings.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new Models.TreeNode
                {
                    title = k.Name,
                    key = k.Code,
                    icon = false,
                    children = null,
                    select = k.Flagged,
                    selected = k.Flagged,
                    parenttitle = k.ParentName,
                    groupLevel = Models.GroupLevel.Site,
                    note = k.Global
                };
                children.Add(child);
            }
            return children;
        }

        private string TempFolder2 => (GetStr(HttpContext.RequestServices.GetService<IConfiguration>()?["temp"]) is string tmp && !string.IsNullOrWhiteSpace(tmp) ? tmp : Common.MapPath("temp")).TrimEnd('\\') + "\\";
        private string DamFolder => (GetStr(HttpContext.RequestServices.GetService<IConfiguration>()?["dam"]) is string tmp && !string.IsNullOrWhiteSpace(tmp) ? tmp : Common.MapPath("DigitalAssets")).TrimEnd('\\') + "\\";
        private string PicNormalFolder => (GetStr(HttpContext.RequestServices.GetService<IConfiguration>()?["picnormal"]) is string tmp && !string.IsNullOrWhiteSpace(tmp) ? tmp : Common.MapPath("picnormal")).TrimEnd('\\') + "\\";
        private string PicThumbnailFolder => (GetStr(HttpContext.RequestServices.GetService<IConfiguration>()?["picthumbnail"]) is string tmp && !string.IsNullOrWhiteSpace(tmp) ? tmp : Common.MapPath("picthumbnail")).TrimEnd('\\') + "\\";
        private string PicOriginalFolder => (GetStr(HttpContext.RequestServices.GetService<IConfiguration>()?["picoriginal"]) is string tmp && !string.IsNullOrWhiteSpace(tmp) ? tmp : Common.MapPath("picoriginal")).TrimEnd('\\') + "\\";

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
                if (delete) File.Delete(strFile);
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

    // Placeholder models & Common - replace with actual
    namespace Models
    {
        public class CategoryData { public CategoryInfo Info { get; set; } = new(); public Profile Profile { get; set; } = new(); public List<CategoryTranslation> Translation { get; set; } = new(); public AutoNumber AutoNumber { get; set; } = new(); public int ActionType { get; set; } public List<int> MergeList { get; set; } = new(); public List<SharingItem> Sharing { get; set; } = new(); }
        public class CategoryInfo { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int Type { get; set; } public bool Global { get; set; } public string Picture { get; set; } = string.Empty; public int Archive { get; set; } }
        public class Profile { public int Code { get; set; } public int CodeSite { get; set; } }
        public class AutoNumber { public int AutoNumberCodeSite { get; set; } public bool AutoNumber { get; set; } public string AutoNumberPrefix { get; set; } = string.Empty; public string AutoNumberStart { get; set; } = string.Empty; public bool AutoNumberKeepPrefixLength { get; set; } }
        public class SharingItem { public int Code { get; set; } }
        public class CategoryTranslation { public string Name { get; set; } = string.Empty; public string Name2 { get; set; } = string.Empty; public int CodeTrans { get; set; } public int CodeSite { get; set; } public string NamePlural { get; set; } = string.Empty; }
        public class GenericCodeValueList { public int Code { get; set; } public string Value { get; set; } = string.Empty; }
        public class Category { public int Code { get; set; } public string Name { get; set; } = string.Empty; public bool Global { get; set; } }
        public class RecipeTranslation { public int CodeTrans { get; set; } public string TranslationName { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public string Picture { get; set; } = string.Empty; public int hasPicture { get; set; } public int Archive { get; set; } }
        public class TreeNode { public string title { get; set; } = string.Empty; public int key { get; set; } public bool icon { get; set; } public List<TreeNode>? children { get; set; } public bool select { get; set; } public bool selected { get; set; } public string parenttitle { get; set; } = string.Empty; public GroupLevel groupLevel { get; set; } public object? note { get; set; } }
        public class GenericTree { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public string ParentName { get; set; } = string.Empty; public bool Flagged { get; set; } public int Type { get; set; } public bool Global { get; set; } }
        public class GenericDeleteData { public List<DeleteCode> CodeList { get; set; } = new(); public int CodeUser { get; set; } public int CodeSite { get; set; } public bool ForceDelete { get; set; } }
        public class DeleteCode { public int Code { get; set; } }
        public class ConfigurationcSearch { public int CodeSite { get; set; } public int CodeTrans { get; set; } public int Type { get; set; } public int Status { get; set; } public int CodeProperty { get; set; } public string Name { get; set; } = string.Empty; }
        public enum GroupLevel { Property, Site }
        public class ResponseCallBack { public int Code { get; set; } public string Message { get; set; } = string.Empty; public object? ReturnValue { get; set; } public bool Status { get; set; } public List<param>? Parameters { get; set; } }
        public class param { public string name { get; set; } = string.Empty; public string value { get; set; } = string.Empty; }
    }

    public static class Common
    {
        public const string SP_API_GET_CategoryInfo = "API_GET_CategoryInfo";
        public static string MapPath(string rel) => rel; // TODO: replace with real mapping
    }
}
