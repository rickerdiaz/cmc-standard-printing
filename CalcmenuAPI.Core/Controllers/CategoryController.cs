using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static EgsData.modGlobalDeclarations; // ConnectionString, DebugEnabled, GroupLevel
using static EgsData.modFunctions;         // GetInt, GetStr, GetBool, Common.Join, Common.SendEmail, Common.MapPath

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private string m_PictureNames = string.Empty;
        private string m_TempPictureNames = string.Empty;
        private string[] arrPictures = Array.Empty<string>();

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpGet("/api/category/{codecategory:int}/{codetrans:int}")]
        public ActionResult<Models.CategoryData> GetCategory(int codecategory, int codetrans)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Common.SP_API_GET_CategoryInfo;
                cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = codecategory;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;

                cmd.Connection.Open();
                using var _da = new SqlDataAdapter(cmd);
                _da.Fill(ds);
                cmd.Connection.Close();

                var categories = new Models.CategoryData();
                // TODO: map ds -> categories if needed
                return Ok(categories);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost("api/category")]
        public ActionResult<Models.ResponseCallBack> SaveCategory([FromBody] Models.CategoryData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            string strCodesToMerge = string.Join(",", data.MergeList ?? new List<int>());

            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing)
                    if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                string codeSiteList = Common.Join(arrSharing, "(", ")", ",");

                bool isarchive;
                bool isactual;
                if (data.Info.Archive == 1) { isactual = true; isarchive = false; }
                else if (data.Info.Archive == 2) { isactual = false; isarchive = true; }
                else { isactual = false; isarchive = false; }

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[MANAGE_CATEGORYUPDATE]";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
                    cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
                    cmd.Parameters.Add("@ListeType", SqlDbType.Int).Value = data.Info.Type;
                    cmd.Parameters.Add("@CodeAcct", SqlDbType.NVarChar, 25).Value = "";
                    cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                    cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 2000).Value = codeSiteList;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                    cmd.Parameters.Add("@IsProduct", SqlDbType.Bit).Value = 0;
                    cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 400).Value = data.Info.Picture ?? string.Empty;
                    cmd.Parameters.Add("@IsActual", SqlDbType.Bit).Value = isactual;
                    cmd.Parameters.Add("@IsArchive", SqlDbType.Bit).Value = isarchive;
                    cmd.Parameters.Add("@CodesToMerge", SqlDbType.NVarChar, 2000).Value = strCodesToMerge ?? string.Empty;
                    var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                    retval.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters["@Code"].Direction = ParameterDirection.InputOutput;

                    cn.Open();
                    _trans = cn.BeginTransaction();
                    cmd.Transaction = _trans;
                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(retval.Value, -1);
                    if (resultCode != 0)
                        throw new DatabaseException($"[{resultCode}] Save category failed");

                    int codeCategory = Convert.ToInt32(cmd.Parameters["@Code"].Value);
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
                            var tr = cmd.Parameters.Add("@retval", SqlDbType.Int);
                            tr.Direction = ParameterDirection.ReturnValue;
                            cmd.ExecuteNonQuery();
                            resultCode = GetInt(tr.Value, -1);
                            if (resultCode != 0)
                                throw new DatabaseException($"[{resultCode}] Save category failed");
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
                            var tr2 = cmd.Parameters.Add("@retval", SqlDbType.Int);
                            tr2.Direction = ParameterDirection.ReturnValue;
                            cmd.ExecuteNonQuery();
                            resultCode = GetInt(tr2.Value, -1);
                            if (resultCode != 0)
                                throw new DatabaseException($"[{resultCode}] Save category failed");
                        }
                    }
                    else
                    {
                        resultCode = GetInt(retval.Value, -1);
                        if (resultCode != 0)
                            throw new DatabaseException($"[{resultCode}] Save category failed");
                    }

                    if (data.ActionType == 5 && (data.MergeList?.Count ?? 0) > 0)
                    {
                        var arrSites = new ArrayList();
                        foreach (var s in data.MergeList!)
                            if (!arrSites.Contains(s)) arrSites.Add(s);
                        string siteList = Common.Join(arrSites, "(", ")", ",");
                        var sql = new StringBuilder();

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

                        var arrMergeList = new ArrayList();
                        foreach (var sh in data.MergeList!)
                            if (!arrMergeList.Contains(sh)) arrMergeList.Add(sh);
                        string mergeList = Common.Join(arrMergeList, "(", ")", ",");

                        sql.Clear();
                        sql.Append("UPDATE EgswListe ");
                        sql.Append("SET Category=@newCategoryCode ");
                        sql.Append(" WHERE Category IN " + mergeList);
                        cmd.CommandText = sql.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@newCategoryCode", SqlDbType.Int).Value = codeCategory;
                        int rowsAffected = cmd.ExecuteNonQuery();

                        cmd.CommandText = "API_DELETE_Generic";
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (var code in arrSites.Cast<int>())
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = GetInt(code);
                            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWCATEGORY";
                            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                            cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = false;
                            cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                            var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                            ret.Direction = ParameterDirection.ReturnValue;
                            cmd.ExecuteNonQuery();
                            resultCode = GetInt(ret.Value, -1);
                            if (resultCode != 0)
                                throw new DatabaseException($"[{resultCode}] Delete merged category failed");
                        }
                    }

                    string procpics = (data.Info.Picture ?? string.Empty) + ";";
                    m_PictureNames = procpics;
                    m_TempPictureNames = data.Info.Picture ?? string.Empty;
                    arrPictures = procpics.Split(';');
                    Log.Info("arrayPictures" + arrPictures.ToString());
                    var thread = new Thread(SavePictures) { Priority = ThreadPriority.Lowest };
                    thread.Start();

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = codeCategory;
                    response.Status = true;
                    _trans.Commit();
                }
                catch (DatabaseException ex)
                {
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                    try { _trans?.Rollback(); (_trans as IDisposable)?.Dispose(); } catch { }
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.Message = "Save category failed";
                    Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Category");
                }
                finally
                {
                    cn.Close(); (cn as IDisposable)?.Dispose();
                }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, aex.Message, aex.StackTrace ?? string.Empty, "Category");
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Category");
            }

            return Ok(response);
        }

        [HttpGet("/api/category/{codesite:int}/{codetrans:int}/{type:int}")]
        public ActionResult<List<Models.GenericCodeValueList>> GetCategoryList(int codesite, int codetrans, int type)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_GET_Generic]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWCATEGORY";
                    cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var categories = new List<Models.GenericCodeValueList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    categories.Add(new Models.GenericCodeValueList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
                return Ok(categories);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/category/{codesite:int}/{codetrans:int}/{type:int}/{status}/{codeproperty?}/{name?}")]
        public ActionResult<List<Models.Category>> SearchCategoryByName(int codesite, int codetrans, int type, int status, int codeproperty = -1, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("dsn")!.ToString());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWCATEGORY";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;

                cmd.Connection.Open();
                using var _da = new SqlDataAdapter(cmd);
                _da.Fill(ds);
                cmd.Connection.Close();

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
                    var categoryresult = new List<Models.Category>();
                    var arrNames = name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            foreach (var c in categories)
                            {
                                if (c.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                                    categoryresult.Add(c);
                            }
                        }
                    }
                    categories = categoryresult;
                }

                return Ok(categories.ToList());
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost("/api/category/search")]
        public ActionResult<List<Models.Category>> SearchCategoryByName2([FromBody] Models.ConfigurationcSearch data)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("dsn")!.ToString());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = data.Type;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = data.Status;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWCATEGORY";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;

                cmd.Connection.Open();
                using var _da = new SqlDataAdapter(cmd);
                _da.Fill(ds);
                cmd.Connection.Close();

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
                    var categoryresult = new List<Models.Category>();
                    var arrNames = data.Name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            foreach (var c in categories)
                            {
                                if (c.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                                    categoryresult.Add(c);
                            }
                        }
                    }
                    categories = categoryresult;
                }

                return Ok(categories.ToList());
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/category/autonumber/{codecategory:int}/{codesite:int}")]
        public ActionResult<Models.AutoNumber> getcategoryautonumber(int codecategory, int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[sp_EgswGetAutoNumberDetailCategory]";
                    cmd.Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = codecategory;
                    cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var autoNumber = new Models.AutoNumber();
                if (ds.Tables[0].Rows.Count < 1) return Ok(autoNumber);

                try
                {
                    var row1 = ds.Tables[0].Rows[0];
                    autoNumber.AutoNumberPrefix = GetStr(row1["Prefix"]);
                    autoNumber.AutoNumber = GetBool(row1["AutoNumber"]);
                    autoNumber.AutoNumberCodeSite = GetInt(row1["CodeSite"]);
                    autoNumber.AutoNumberKeepPrefixLength = GetBool(row1["KeepLength"]);
                    autoNumber.AutoNumberStart = GetStr(row1["StartingNumber"]);
                    return Ok(autoNumber);
                }
                catch (Exception ex)
                {
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                    return Problem(title: "Request failed", statusCode: 500);
                }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/autonumber/{codesite:int}/{codeuser:int}/{type:int}/{category:int?}")]
        public ActionResult<Models.ResponseCallBack> GetAutoNumber(int codesite, int codeuser, int type, int category = -1)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                string strNumber = string.Empty;
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_GET_AutoNumber]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = category;
                    var number = cmd.Parameters.Add("@Number", SqlDbType.VarChar, 50);
                    number.Direction = ParameterDirection.Output;
                    var err = cmd.Parameters.Add("@ERR", SqlDbType.Int);
                    err.Direction = ParameterDirection.ReturnValue;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    strNumber = GetStr(number.Value);
                    resultCode = GetInt(err.Value);
                    if (resultCode != 0)
                        throw new DatabaseException($"[{resultCode}] Get Autonumber failed");

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = strNumber;
                    response.Status = true;
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
            }
            return Ok(response);
        }

        [HttpGet("/api/category/translation/{codecategory:int}/{codesite:int}")]
        public ActionResult<List<Models.RecipeTranslation>> GetCategoryTranslation(int codecategory, int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_GET_CategoryTranslation]";
                    cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = codecategory;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                bool isArchive;
                bool isActual;
                int archive;

                var translations = new List<Models.RecipeTranslation>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    isArchive = Convert.ToBoolean(r["isArchive"]);
                    isActual = Convert.ToBoolean(r["isActual"]);
                    archive = isArchive ? 2 : (isActual ? 1 : 0);
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

                return Ok(translations.ToList());
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost("api/category/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteCategory([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                var arrCategoryCodes = new ArrayList();
                foreach (var c in data.CodeList)
                    if (!arrCategoryCodes.Contains(c.Code)) arrCategoryCodes.Add(c.Code);
                string codeCategoryList = Common.Join(arrCategoryCodes, "", "", ",");

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
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
                    if (resultCode != 0)
                        throw new DatabaseException($"[{resultCode}] Delete category failed");

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = GetStr(skipList.Value);
                    response.Status = true;
                }
                catch (DatabaseException ex)
                {
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                    response.Message = "Delete category failed";
                    Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Category");
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, aex.Message, aex.StackTrace ?? string.Empty, "Category");
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Category");
            }
            return Ok(response);
        }

        [HttpPost("api/category/purge/{type:int}/{codeUser:int}/{codeSite:int}")]
        public ActionResult<Models.ResponseCallBack> PurgeCategory(int type, long codeUser, long codeSite)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;

            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + "[" + type.ToString() + "]");

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
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
                    _trans = cn.BeginTransaction();
                    cmd.Transaction = _trans;
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
                            throw new DatabaseException($"[{resultCode}] Purge category failed");
                        }
                    }
                    else
                    {
                        response.Code = 0;
                        response.Message = "OK";
                        response.Status = true;
                    }

                    _trans.Commit();
                }
                catch (DatabaseException ex)
                {
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                    try { _trans?.Rollback(); (_trans as IDisposable)?.Dispose(); } catch { }
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.Message = "Purge category failed";
                    Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Category");
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, aex.Message, aex.StackTrace ?? string.Empty, "Category");
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Category");
            }
            return Ok(response);
        }

        [HttpGet("/api/category/sharing/{codesite:int}/{type:int}/{tree:int}/{codecategory:int}")]
        public ActionResult<List<Models.TreeNode>> GetCategorySharing(int codesite, int type, int tree, int codecategory)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "[dbo].[API_GET_SharingCategory]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = codecategory;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var sharings = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new Models.GenericTree
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

                var sharingdata = new List<Models.TreeNode>();
                var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildrenSharing(sharings, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        groupLevel = GroupLevel.Property
                    };
                    if (parent.children != null && parent.children.Count > 0) sharingdata.Add(parent);
                }

                return Ok(sharingdata);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/category/sharingMerge/{codesite:int}/{type:int}/{tree:int}/{codecategory?}")]
        public ActionResult<List<Models.TreeNode>> GetCategorySharingMerge(int codesite, int type, int tree, string codecategory)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "[dbo].[API_GET_SharingMerge]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeCategory", SqlDbType.NVarChar, 50).Value = codecategory;
                    cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 19;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var sharings = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new Models.GenericTree
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

                var sharingdata = new List<Models.TreeNode>();
                var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildrenSharing(sharings, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        groupLevel = GroupLevel.Property
                    };
                    if (parent.children != null && parent.children.Count > 0) sharingdata.Add(parent);
                }

                return Ok(sharingdata);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> sharingdata, int code)
        {
            var children = new List<Models.TreeNode>();
            if (sharingdata != null)
            {
                var kids = sharingdata.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
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
                        groupLevel = GroupLevel.Site,
                        note = k.Global
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        private void SavePictures()
        {
            try
            {
                string strTempFolder = TempFolder2;
                if (!string.IsNullOrWhiteSpace(m_PictureNames))
                {
                    var arrPictureNames = m_PictureNames.Trim().Split(';');
                    m_PictureNames = string.Join(";", arrPictures);
                    if (arrPictureNames.Length > 0)
                    {
                        for (int ctr = 0; ctr < arrPictureNames.Length; ctr++)
                        {
                            if (!string.IsNullOrWhiteSpace(arrPictureNames[ctr]))
                            {
                                string pic = arrPictureNames[ctr];
                                string _source;
                                if (pic.Trim().IndexOf("| DAM", StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    pic = pic.Substring(0, pic.IndexOf("|", StringComparison.Ordinal)).Trim();
                                    _source = DamFolder + pic;
                                    File.Copy(_source, strTempFolder + arrPictures[ctr], true);
                                    pic = arrPictures[ctr];
                                }
                                _source = strTempFolder + pic;
                                if (File.Exists(_source))
                                {
                                    File.Copy(_source, PicOriginalFolder + pic, true);
                                    fctResizeConvertImage(_source, PicNormalFolder + pic, 300, 300, false);
                                    fctResizeConvertImage(_source, PicThumbnailFolder + pic, 200, 200, false);
                                    Log.Info("Picture saved:" + _source);
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(m_TempPictureNames))
                {
                    var arrPictureNames = m_TempPictureNames.Trim().Split(';');
                    if (arrPictureNames.Length > 0)
                    {
                        foreach (var pic in arrPictureNames)
                        {
                            if (!string.IsNullOrWhiteSpace(pic))
                            {
                                if (pic.Trim().LastIndexOf("| DAM", StringComparison.OrdinalIgnoreCase) == -1)
                                {
                                    string _source = strTempFolder + pic;
                                    if (File.Exists(_source)) File.Delete(_source);

                                    if (m_PictureNames.IndexOf(pic, StringComparison.OrdinalIgnoreCase) == -1)
                                    {
                                        if (File.Exists(PicOriginalFolder + pic)) File.Delete(PicOriginalFolder + pic);
                                        if (File.Exists(PicNormalFolder + pic)) File.Delete(PicNormalFolder + pic);
                                        if (File.Exists(PicThumbnailFolder + pic)) File.Delete(PicThumbnailFolder + pic);
                                        Log.Info("Picture deleted:" + pic);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Save pictures failed", ex);
            }
        }

        public string TempFolder2
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["temp"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("temp");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string DamFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["dam"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("DigitalAssets");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicNormalFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["picnormal"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picnormal");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicThumbnailFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["picthumbnail"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picthumbnail");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicOriginalFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["picoriginal"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picoriginal");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        private bool fctResizeConvertImage(string strFile, string strDestination, int newWidth, int newHeight, bool blnDelete = false)
        {
            try
            {
                double dblTempW;
                double dblTempH;
                int H1;
                int W1;

                string FileToResize = strFile;
                using var originalBitmap = (Bitmap)Bitmap.FromFile(FileToResize, true);
                decimal WidthVsHeightRatio = (decimal)originalBitmap.Height / (decimal)originalBitmap.Width;

                if (newHeight > newWidth)
                {
                    dblTempH = newHeight;
                    dblTempW = dblTempH / (double)WidthVsHeightRatio;
                }
                else
                {
                    dblTempW = newWidth;
                    dblTempH = dblTempW * (double)WidthVsHeightRatio;
                }
                while (dblTempW > newWidth || dblTempH > newHeight)
                {
                    dblTempW *= 0.999;
                    dblTempH *= 0.999;
                }
                W1 = (int)dblTempW;
                H1 = (int)dblTempH;

                using var newbmp = new Bitmap(W1, H1);
                using (var newg = Graphics.FromImage(newbmp))
                {
                    newg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    newg.Clear(Color.White);
                    newg.DrawImage(originalBitmap, 0, 0, W1, H1);
                }
                newbmp.Save(strDestination, System.Drawing.Imaging.ImageFormat.Jpeg);
                if (blnDelete && File.Exists(strFile)) File.Delete(strFile);
            }
            catch (Exception ex)
            {
                Log.Error("ResizeConvertImage failed", ex);
                return false;
            }
            return true;
        }
    }
}
