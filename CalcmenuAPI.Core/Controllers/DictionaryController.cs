using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static EgsData.modGlobalDeclarations; // ConnectionString, GroupLevel, DebugEnabled
using static EgsData.modFunctions;         // GetInt, GetStr, GetBool, Common.ReplaceSpecialCharacters, Common.Join, Common.SendEmail

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpPost("/api/dictionary/search")]
        public ActionResult<List<Models.Dictionary>> GetDictionaryByName2([FromBody] Models.ConfigurationcSearch data)
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
                    cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
                    cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWDICTIONARY";
                    cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

                var dictionary = new List<Models.Dictionary>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new Models.Dictionary
                    {
                        CodeGroup = GetInt(r["CodeGroup"]),
                        Name = GetStr(r["Name"]),
                        CodeDictionary = GetInt(r["CodeDictionary"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var dictionaryresult = new List<Models.Dictionary>();
                    var arrNames = data.Name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            foreach (var s in dictionary)
                            {
                                if (s.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                                    dictionaryresult.Add(s);
                            }
                        }
                    }
                    dictionary = dictionaryresult;
                }

                return Ok(dictionary.ToList());
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

        [HttpGet("/api/dictionary/sharing/{codesite:int}/{type:int}/{tree:int}/{codedictionary:int}")]
        public ActionResult<List<Models.TreeNode>> GetdictionarySharing(int codesite, int type, int tree, int codedictionary)
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
                    cmd.CommandText = "[dbo].[API_GET_SharingAll]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codedictionary;
                    cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 117;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

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
                    if (!sharingdata.Any(obj => obj.key == p.Code))
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

        [HttpGet("/api/dictionary/translation/{code:int}/{codesite:int}")]
        public ActionResult<List<Models.RecipeTranslation>> GetDictionaryTranslation(int code, int codesite)
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
                    cmd.CommandText = "[dbo].[API_GET_DictionaryTranslation]";
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

                bool isArchive;
                bool isActual;
                int archive;
                var translations = new List<Models.RecipeTranslation>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    // VB code had isArchive/isActual without reading; keeping archive = 0
                    archive = 0;
                    translations.Add(new Models.RecipeTranslation
                    {
                        CodeTrans = GetInt(r["CodeTrans"]),
                        TranslationName = GetStr(r["TranslationName"]),
                        Name = GetStr(r["Name"]) 
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

        [HttpPost("/api/dictionary")]
        public ActionResult<Models.ResponseCallBack> SaveDictionary([FromBody] Models.DictionaryData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cn.Open();
                    int codeDictionary = data.Info.CodeDictionary;

                    cmd.CommandText = "MANAGE_DICTIONARYVALIDATION";
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var t in data.Translation)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = data.Info.CodeGroup;
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 600).Value = t.Name;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                        cmd.Parameters.Add("@CodeDictionary", SqlDbType.Int).Value = codeDictionary;
                        var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                        retval.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(retval.Value, -1);
                        if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save dictionary failed");
                    }

                    codeDictionary = data.Info.CodeDictionary;
                    cmd.CommandText = "MANAGE_DICTIONARYUPDATE";
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var t in data.Translation)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = data.Info.CodeGroup;
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 600).Value = t.Name;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                        cmd.Parameters.Add("@CodeTransMain", SqlDbType.Int).Value = data.Info.CodeTransMain;
                        var codeParam = cmd.Parameters.Add("@CodeDictionary", SqlDbType.Int);
                        codeParam.Value = codeDictionary;
                        codeParam.Direction = ParameterDirection.InputOutput;
                        var retval2 = cmd.Parameters.Add("@retval", SqlDbType.Int);
                        retval2.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        codeDictionary = GetInt(codeParam.Value, -1);
                        resultCode = GetInt(retval2.Value, -1);
                        if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save dictionary failed");
                    }

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = codeDictionary;
                    response.Status = true;
                }
                catch (DatabaseException ex)
                {
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.Message = "Save dictionary failed";
                    Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Dictionary");
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, aex.Message, aex.StackTrace ?? string.Empty, "Dictionary");
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Dictionary");
            }
            return Ok(response);
        }

        [HttpPost("/api/dictionary/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteDictionary([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                var arrDictionaryCodes = new ArrayList();
                foreach (var c in data.CodeList)
                    if (!arrDictionaryCodes.Contains(c.CodeDictionary)) arrDictionaryCodes.Add(c.CodeDictionary);
                string codeDictionaryList = Common.Join(arrDictionaryCodes, "", "", ",");

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "API_DELETE_Generic";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeDictionaryList;
                    cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWDICTIONARY";
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
                    if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete dictionary failed");

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
                    response.Message = "Delete dictionary failed";
                    Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Dictionary");
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, aex.Message, aex.StackTrace ?? string.Empty, "Dictionary");
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Dictionary");
            }
            return Ok(response);
        }

        [HttpGet("/api/dictionary/{codesite:int}/{codetrans:int}/{codegroup:int}")]
        public ActionResult<List<Models.GenericList>> GetDictionaryList(int codesite, int codetrans, int codegroup)
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
                    cmd.CommandText = "sp_PMDictionaryGetList";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = codegroup;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

                var dictionary = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new Models.GenericList { Code = GetInt(r["CodeDictionary"]), Value = GetStr(r["Name"]) });
                }
                return Ok(dictionary);
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

        [HttpGet("/api/dictionary/{codesite:int}/{codetrans:int}/{codegroup:int}/{codegroup2:int}")]
        public ActionResult<List<Models.GenericList>> GetDictionaryListAll(int codesite, int codetrans, int codegroup, int codegroup2)
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
                    cmd.CommandText = "sp_PMDictionaryGetListALL";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = codegroup;
                    cmd.Parameters.Add("@CodeGroup2", SqlDbType.Int).Value = codegroup2;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

                var dictionary = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new Models.GenericList { Code = GetInt(r["CodeDictionary"]), Value = GetStr(r["Name"]) });
                }
                return Ok(dictionary);
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

        [HttpGet("/api/manordictionary/{codetrans:int}/{codegroup:int}")]
        public ActionResult<List<Models.GenericList>> GetDictionaryList_Manor(int codetrans, int codegroup)
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
                    cmd.CommandText = "sp_ManorGetDictionaryList";
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = codegroup;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

                var dictionary = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new Models.GenericList { Code = GetInt(r["CodeDictionary"]), Value = GetStr(r["Name"]) });
                }
                return Ok(dictionary);
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
    }
}
