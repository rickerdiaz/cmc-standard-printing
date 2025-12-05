using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController : ControllerBase
    {
        // TODO: Wire real logger
        private static void LogInfo(string msg) { /* hook log4net or ILogger */ }
        private static void LogWarn(string msg, Exception ex) { /* hook log4net or ILogger */ }
        private static void LogError(string msg, Exception ex) { /* hook log4net or ILogger */ }

        // TODO: Provide actual connection string through configuration
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        // TODO: Port helpers from VB/Common module
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

        // POST /api/dictionary/search
        [HttpPost("search")]
        public ActionResult<List<Models.Dictionary>> GetDictionaryByName2([FromBody] Models.ConfigurationcSearch data)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
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
                        using var da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }

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
                        var w = (word ?? string.Empty).Trim();
                        if (w.Length == 0) continue;
                        foreach (var s in dictionary)
                        {
                            if (s.Name?.ToLowerInvariant().Contains(w.ToLowerInvariant()) == true)
                            {
                                dictionaryresult.Add(s);
                            }
                        }
                    }
                    dictionary = dictionaryresult;
                }

                return Ok(dictionary);
            }
            catch (ArgumentException aex)
            {
                LogWarn("GetDictionaryByName2: Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                LogError("GetDictionaryByName2: Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        // GET /api/dictionary/sharing/{codesite}/{type}/{tree}/{codedictionary}
        [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codedictionary:int}")]
        public ActionResult<List<Models.TreeNode>> GetdictionarySharing(int codesite, int type, int tree, int codedictionary)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
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
                        using var da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }

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
                    if (sharingdata.All(obj => obj.key != p.Code))
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
                            groupLevel = Models.GroupLevel.Property
                        };
                        if (parent.children.Count > 0)
                            sharingdata.Add(parent);
                    }
                }

                return Ok(sharingdata);
            }
            catch (ArgumentException aex)
            {
                LogWarn("GetdictionarySharing: Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                LogError("GetdictionarySharing: Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> sharings, int parentCode)
        {
            var children = sharings.Where(s => s.ParentCode == parentCode).OrderBy(s => s.Name).ToList();
            var result = new List<Models.TreeNode>();
            foreach (var c in children)
            {
                var node = new Models.TreeNode
                {
                    title = c.Name,
                    key = c.Code,
                    icon = false,
                    children = CreateChildrenSharing(sharings, c.Code),
                    select = c.Flagged,
                    selected = c.Flagged,
                    parenttitle = c.ParentName,
                    groupLevel = Models.GroupLevel.Property
                };
                result.Add(node);
            }
            return result;
        }

        // GET /api/dictionary/translation/{code}/{codesite}
        [HttpGet("translation/{code:int}/{codesite:int}")]
        public ActionResult<List<Models.RecipeTranslation>> GetDictionaryTranslation(int code, int codesite)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_DictionaryTranslation]";
                        cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cn.Open();
                        using var da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }

                var translations = new List<Models.RecipeTranslation>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    translations.Add(new Models.RecipeTranslation
                    {
                        CodeTrans = GetInt(r["CodeTrans"]),
                        TranslationName = GetStr(r["TranslationName"]),
                        Name = GetStr(r["Name"])
                    });
                }

                return Ok(translations);
            }
            catch (ArgumentException aex)
            {
                LogWarn("GetDictionaryTranslation: Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                LogError("GetDictionaryTranslation: Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        // POST api/dictionary
        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SaveDictionary([FromBody] Models.DictionaryData data)
        {
            var response = new Models.ResponseCallBack();
            var resultCode = 0;
            try
            {
                LogInfo("SaveDictionary: " + JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cn.Open();
                        var codeDictionary = data.Info.CodeDictionary;

                        cmd.CommandText = "MANAGE_DICTIONARYVALIDATION";
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (var t in data.Translation)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = data.Info.CodeGroup;
                            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 600).Value = t.Name;
                            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                            cmd.Parameters.Add("@CodeDictionary", SqlDbType.Int).Value = codeDictionary;
                            var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
                            ret.Direction = ParameterDirection.ReturnValue;
                            cmd.ExecuteNonQuery();
                            resultCode = GetInt(ret.Value, -1);
                            if (resultCode != 0)
                            {
                                throw new Exception($"[{resultCode}] Save dictionary failed");
                            }
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
                            var pCodeDict = cmd.Parameters.Add("@CodeDictionary", SqlDbType.Int);
                            pCodeDict.Direction = ParameterDirection.InputOutput;
                            pCodeDict.Value = codeDictionary;
                            var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
                            ret.Direction = ParameterDirection.ReturnValue;
                            cmd.ExecuteNonQuery();
                            codeDictionary = GetInt(pCodeDict.Value, -1);
                            resultCode = GetInt(ret.Value, -1);
                            if (resultCode != 0)
                            {
                                throw new Exception($"[{resultCode}] Save dictionary failed");
                            }
                        }

                        response.Code = 0;
                        response.Message = "OK";
                        response.ReturnValue = codeDictionary;
                        response.Status = true;
                    }
                    catch (Exception dbEx)
                    {
                        LogError("SaveDictionary: Database error occured", dbEx);
                        if (resultCode == 0) resultCode = 500;
                        response.Code = resultCode;
                        response.Status = false;
                        response.Message = "Save dictionary failed";
                        return StatusCode(500, response);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            }
            catch (ArgumentException aex)
            {
                LogWarn("SaveDictionary: Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                LogError("SaveDictionary: Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        // POST api/dictionary/delete
        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteDictionary([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            var resultCode = 0;
            try
            {
                LogInfo("DeleteDictionary: " + JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                var arrDictionaryCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrDictionaryCodes.Contains(c.CodeDictionary))
                        arrDictionaryCodes.Add(c.CodeDictionary);
                }
                var codeList = string.Join(",", arrDictionaryCodes.Cast<object>().Select(x => Convert.ToString(x)));

                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = "API_DELETE_Generic";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
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
                        if (resultCode != 0)
                        {
                            throw new Exception($"[{resultCode}] Delete dictionary failed");
                        }

                        response.Code = 0;
                        response.Message = "OK";
                        response.ReturnValue = GetStr(skipList.Value);
                        response.Status = true;
                    }
                    catch (Exception dbEx)
                    {
                        LogError("DeleteDictionary: Database error occured", dbEx);
                        if (resultCode == 0) resultCode = 500;
                        response.Code = resultCode;
                        response.Status = false;
                        response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                        response.Message = "Delete dictionary failed";
                        return StatusCode(500, response);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            }
            catch (ArgumentException aex)
            {
                LogWarn("DeleteDictionary: Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                LogError("DeleteDictionary: Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        // GET /api/dictionary/{codesite}/{codetrans}/{codegroup}
        [HttpGet("{codesite:int}/{codetrans:int}/{codegroup:int}")]
        public ActionResult<List<Models.GenericList>> GetDictionaryList(int codesite, int codetrans, int codegroup)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sp_PMDictionaryGetList";
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                        cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = codegroup;
                        cn.Open();
                        using var da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }

                var dictionary = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new Models.GenericList
                    {
                        Code = GetInt(r["CodeDictionary"]),
                        Value = GetStr(r["Name"]) 
                    });
                }

                return Ok(dictionary);
            }
            catch (ArgumentException aex)
            {
                LogWarn("GetDictionaryList: Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                LogError("GetDictionaryList: Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        // GET /api/dictionary/{codesite}/{codetrans}/{codegroup}/{codegroup2}
        [HttpGet("{codesite:int}/{codetrans:int}/{codegroup:int}/{codegroup2:int}")]
        public ActionResult<List<Models.GenericList>> GetDictionaryListAll(int codesite, int codetrans, int codegroup, int codegroup2)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
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
                        using var da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }

                var dictionary = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new Models.GenericList
                    {
                        Code = GetInt(r["CodeDictionary"]),
                        Value = GetStr(r["Name"]) 
                    });
                }

                return Ok(dictionary);
            }
            catch (ArgumentException aex)
            {
                LogWarn("GetDictionaryListAll: Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                LogError("GetDictionaryListAll: Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }
    }

    // NOTE: Placeholder namespace for Models to compile; replace with actual project models namespace
    namespace Models
    {
        public enum GroupLevel { Property }
        public class Dictionary { public int CodeGroup { get; set; } public string? Name { get; set; } public int CodeDictionary { get; set; } }
        public class ConfigurationcSearch { public int CodeSite { get; set; } public int CodeTrans { get; set; } public int CodeProperty { get; set; } public string Name { get; set; } = string.Empty; }
        public class GenericTree { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public string ParentName { get; set; } = string.Empty; public bool Flagged { get; set; } public int Type { get; set; } public bool Global { get; set; } }
        public class TreeNode { public string title { get; set; } = string.Empty; public int key { get; set; } public bool icon { get; set; } public List<TreeNode> children { get; set; } = new(); public bool select { get; set; } public bool selected { get; set; } public string parenttitle { get; set; } = string.Empty; public GroupLevel groupLevel { get; set; } }
        public class RecipeTranslation { public int CodeTrans { get; set; } public string TranslationName { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; }
        public class DictionaryData { public DictionaryInfo Info { get; set; } = new(); public List<DictionaryTranslation> Translation { get; set; } = new(); }
        public class DictionaryInfo { public int CodeDictionary { get; set; } public int CodeGroup { get; set; } public int CodeTransMain { get; set; } }
        public class DictionaryTranslation { public string Name { get; set; } = string.Empty; public int CodeTrans { get; set; } }
        public class ResponseCallBack { public int Code { get; set; } public string Message { get; set; } = string.Empty; public object? ReturnValue { get; set; } public bool Status { get; set; } public List<param>? Parameters { get; set; } }
        public class param { public string name { get; set; } = string.Empty; public string value { get; set; } = string.Empty; }
        public class GenericDeleteData { public List<DeleteCode> CodeList { get; set; } = new(); public int CodeUser { get; set; } public int CodeSite { get; set; } public bool ForceDelete { get; set; } }
        public class DeleteCode { public int CodeDictionary { get; set; } }
        public class GenericList { public int Code { get; set; } public string Value { get; set; } = string.Empty; }
    }
}
