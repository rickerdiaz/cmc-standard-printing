using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KeywordController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codesite:int}/{codetrans:int}/{type:int}")]
        public ActionResult<List<Models.GenericTree>> GetKeyword(int codesite, int codetrans, int type)
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

                var keywords = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    keywords.Add(new Models.GenericTree
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
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.TreeNode>> GetKeywordByName(int codesite, int codetrans, int type, int tree, int codeliste, int codeproperty = -1, string? name = "")
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

                var keywords = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    keywords.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Note = GetStr(r["Inheritable"]) 
                    });
                }

                var keyworddata = new List<Models.TreeNode>();
                var parents = keywords.Where(obj => obj.ParentCode <= 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (keyworddata.All(obj => obj.key != p.Code))
                    {
                        var parent = new Models.TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateChildren(keywords, p.Code),
                            select = p.Flagged,
                            selected = p.Flagged,
                            parenttitle = p.ParentName,
                            note = p.Note
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
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost("search")]
        public ActionResult<List<Models.TreeNode>> GetKeywordByName2([FromBody] Models.ConfigurationcSearch data)
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

                var keywords = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    keywords.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Note = GetStr(r["Inheritable"]) 
                    });
                }

                var keyworddata = new List<Models.TreeNode>();
                var parents = keywords.Where(obj => obj.ParentCode <= 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (keyworddata.All(obj => obj.key != p.Code))
                    {
                        var parent = new Models.TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateChildren(keywords, p.Code),
                            select = p.Flagged,
                            parenttitle = p.ParentName,
                            note = p.Note
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
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("list/{codesite:int}/{codetrans:int}/{type:int}")]
        public ActionResult<List<Models.GenericList>> GetKeywordList(int codesite, int codetrans, int type)
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
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = -1;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var keywords = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    keywords.Add(new Models.GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
                return Ok(keywords);
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

        [HttpGet("translation/{codekeyword:int}/{codesite:int}")]
        public ActionResult<List<Models.RecipeTranslation>> GetKeywordTranslation(int codekeyword, int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "API_GET_KeywordTranslation";
                cmd.Parameters.Add("@CodeKeyword", SqlDbType.Int).Value = codekeyword;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

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
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("sharing/{codekeyword:int}")]
        public ActionResult<List<Models.TreeNode>> GetKeywordSharing(int codekeyword)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_SharingKeyword]";
                cmd.Parameters.Add("@CodeKeyword", SqlDbType.Int).Value = codekeyword;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

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
                        groupLevel = Models.GroupLevel.Property
                    };
                    if (parent.children.Count > 0) sharingdata.Add(parent);
                }
                return Ok(sharingdata);
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
        public ActionResult<Models.ResponseCallBack> SaveKeyword([FromBody] Models.KeywordData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? trans = null;
            string strCodesToMerge = string.Empty;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing) if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                var codeSiteList = string.Join(",", arrSharing.Cast<object>());
                if (data.MergeList.Count > 0)
                {
                    foreach (var s in data.MergeList) strCodesToMerge = string.IsNullOrEmpty(strCodesToMerge) ? s.ToString() : strCodesToMerge + "," + s;
                }

                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "API_Manage_Keyword_Update";
                var pCode = cmd.Parameters.Add("@Code", SqlDbType.Int);
                pCode.Value = data.Info.Code;
                pCode.Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
                cmd.Parameters.Add("@Parent", SqlDbType.Int).Value = data.Info.ParentCode;
                cmd.Parameters.Add("@ListeType", SqlDbType.Int).Value = data.Info.Type;
                cmd.Parameters.Add("@IsInheritable", SqlDbType.Bit).Value = data.Info.Inheritable;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 2000).Value = string.Empty;
                cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 2000).Value = codeSiteList;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Info.CodeSite;
                cmd.Parameters.Add("@CodesToMerge", SqlDbType.NVarChar, 2000).Value = strCodesToMerge;
                var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
                var codeKeyword = Convert.ToInt32(pCode.Value);
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0) throw new Exception($"[{resultCode}] Save keyword failed");

                if (codeKeyword > -1)
                {
                    foreach (var t in data.Translation)
                    {
                        cmd.CommandText = "sp_EgswItemTranslationUpdate";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codeKeyword;
                        cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = t.Name;
                        cmd.Parameters.Add("@nvcName2", SqlDbType.NVarChar, 150).Value = t.Name2;
                        cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                        cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = t.CodeSite;
                        cmd.Parameters.Add("@tntListType", SqlDbType.Int).Value = 6;
                        cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                        cmd.Parameters.Add("@tntType", SqlDbType.Int).Value = data.Info.Type;
                        cmd.Parameters.Add("@nvcPlural", SqlDbType.NVarChar, 150).Value = t.NamePlural;
                        var retTr = cmd.Parameters.Add("@retval", SqlDbType.Int);
                        retTr.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(retTr.Value, -1);
                        if (resultCode != 0) throw new Exception($"[{resultCode}] Update keyword translation failed");
                    }

                    foreach (var kiosk in data.KioskList)
                    {
                        cmd.CommandText = "DELETE_KeywordsBrandSite";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeKeyword", SqlDbType.Int, 4).Value = codeKeyword;
                        cmd.Parameters.Add("@CodeBrandSite", SqlDbType.Int, 4).Value = kiosk.Code;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "UPDATE_KeywordsBrandSite";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeKeyword", SqlDbType.Int, 4).Value = codeKeyword;
                        cmd.Parameters.Add("@CodeBrandSite", SqlDbType.Int, 4).Value = kiosk.Code;
                        cmd.ExecuteNonQuery();
                    }

                    if (data.KioskList.Count == 0)
                    {
                        cmd.CommandText = $"DELETE FROM EgswBrandSiteKeywords WHERE CodeKeyword={codeKeyword}";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.ExecuteNonQuery();
                    }
                }

                if (data.ActionType == 5 && data.MergeList.Count > 0)
                {
                    var arrSites = new ArrayList();
                    foreach (var s in data.MergeList) if (!arrSites.Contains(s)) arrSites.Add(s);
                    var mergeList = "(" + string.Join(",", arrSites.Cast<object>()) + ")";

                    var sql = $"Delete from EgswSharing WHERE Code IN {mergeList} AND CodeEgswTable=43 AND TYPE={data.Info.Type}";
                    cmd.CommandText = sql; cmd.CommandType = CommandType.Text; cmd.Parameters.Clear(); cmd.ExecuteNonQuery();

                    sql = $"UPDATE EgswKeyword SET Parent=@Code where Parent IN {mergeList} AND Type={data.Info.Type}";
                    cmd.CommandText = sql; cmd.CommandType = CommandType.Text; cmd.Parameters.Clear(); cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codeKeyword; cmd.ExecuteNonQuery();

                    sql = $"DECLARE @MyTable Table ( CodeListe int, Derived int );  INSERT INTO @MyTable SELECT   CodeListe, MIN( CONVERT(INT, Derived) ) FROM EgswKeyDetails  WHERE CodeKey in {mergeList} GROUP BY CodeListe;  DELETE FROM EgswKeyDetails WHERE CodeKey in {mergeList}; INSERT INTO EgswKeyDetails (CodeListe, CodeKey, Derived) SELECT a.CodeListe, @newCode , a.Derived From @MyTable a;";
                    cmd.CommandText = sql; cmd.CommandType = CommandType.Text; cmd.Parameters.Clear(); cmd.Parameters.Add("@newCode", SqlDbType.Int).Value = codeKeyword; cmd.ExecuteNonQuery();

                    cmd.CommandText = "API_DELETE_Generic"; cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var code in arrSites.Cast<int>())
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = code;
                        cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWKEYWORD";
                        cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                        cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = false;
                        var skip = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000); skip.Direction = ParameterDirection.Output;
                        var retDel = cmd.Parameters.Add("@Return", SqlDbType.Int); retDel.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(retDel.Value, -1);
                        if (resultCode != 0) throw new Exception($"[{resultCode}] Delete merged keyword failed");
                    }
                }

                trans.Commit();
                response.Code = 0; response.Message = "OK"; response.ReturnValue = Convert.ToInt32(pCode.Value); response.Status = true;
            }
            catch (Exception)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500; response.Code = resultCode; response.Status = false; response.Message = "Save keyword failed"; return StatusCode(500, response);
            }
            return Ok(response);
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteKeyword([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrKeywordCodes = new ArrayList();
                foreach (var c in data.CodeList) if (!arrKeywordCodes.Contains(c.Code)) arrKeywordCodes.Add(c.Code);
                var codeKeywordList = string.Join(",", arrKeywordCodes.Cast<object>());

                cmd.Connection = cn; cmd.CommandType = CommandType.StoredProcedure; cmd.CommandText = "API_DELETE_Generic";
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeKeywordList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWKEYWORD";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                var skip = cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000); skip.Direction = ParameterDirection.Output;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int); ret.Direction = ParameterDirection.ReturnValue;
                cn.Open(); cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0) throw new Exception($"[{resultCode}] Delete keyword failed");
                response.Code = 0; response.Message = "OK"; response.ReturnValue = GetStr(skip.Value); response.Status = true;
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500; response.Code = resultCode; response.Status = false; response.ReturnValue = string.Empty; response.Message = "Delete keyword failed"; return StatusCode(500, response);
            }
            return Ok(response);
        }

        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> data, int code)
        {
            var children = new List<Models.TreeNode>();
            var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new Models.TreeNode
                {
                    title = k.Name,
                    key = k.Code,
                    icon = false,
                    children = CreateChildren(data, k.Code),
                    select = k.Flagged,
                    selected = k.Flagged,
                    parenttitle = k.ParentName,
                    note = k.Note
                };
                children.Add(child);
            }
            return children;
        }

        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> data, int code)
        {
            var children = new List<Models.TreeNode>();
            var kids = data.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
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

        private static int GetInt(object? value, int fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (int.TryParse(Convert.ToString(value), out var i)) return i; try { return Convert.ToInt32(value); } catch { return fallback; } }
        private static string GetStr(object? value) { return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty; }
        private static bool GetBool(object? value) { if (value == null || value == DBNull.Value) return false; if (bool.TryParse(Convert.ToString(value), out var b)) return b; try { return Convert.ToInt32(value) != 0; } catch { return false; } }
    }

    // Placeholder models & Common - replace with actual
    namespace Models
    {
        public class GenericTree { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public string ParentName { get; set; } = string.Empty; public bool Flagged { get; set; } public int Type { get; set; } public bool Global { get; set; } public string Note { get; set; } = string.Empty; public bool CanBeParent { get; set; } }
        public class TreeNode { public string title { get; set; } = string.Empty; public int key { get; set; } public bool icon { get; set; } public List<TreeNode>? children { get; set; } public bool select { get; set; } public bool selected { get; set; } public string parenttitle { get; set; } = string.Empty; public GroupLevel groupLevel { get; set; } public object? note { get; set; } public int ParentCode { get; set; } public bool CanBeParent { get; set; } }
        public class GenericList { public int Code { get; set; } public string Value { get; set; } = string.Empty; }
        public class RecipeTranslation { public int CodeTrans { get; set; } public string TranslationName { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; }
        public class KeywordData { public KeywordInfo Info { get; set; } = new(); public Profile Profile { get; set; } = new(); public List<TranslationItem> Translation { get; set; } = new(); public List<SharingItem> Sharing { get; set; } = new(); public List<GenericList> KioskList { get; set; } = new(); public int ActionType { get; set; } public List<int> MergeList { get; set; } = new(); }
        public class KeywordInfo { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public int Type { get; set; } public bool Inheritable { get; set; } public bool Global { get; set; } public int CodeSite { get; set; } }
        public class Profile { public int Code { get; set; } public int CodeSite { get; set; } }
        public class TranslationItem { public string Name { get; set; } = string.Empty; public string Name2 { get; set; } = string.Empty; public int CodeTrans { get; set; } public int CodeSite { get; set; } public string NamePlural { get; set; } = string.Empty; }
        public class SharingItem { public int Code { get; set; } public string Value { get; set; } = string.Empty; }
        public class ResponseCallBack { public int Code { get; set; } public string Message { get; set; } = string.Empty; public object? ReturnValue { get; set; } public bool Status { get; set; } public List<param>? Parameters { get; set; } }
        public class param { public string name { get; set; } = string.Empty; public string value { get; set; } = string.Empty; }
        public class ConfigurationcSearch { public int CodeSite { get; set; } public int CodeTrans { get; set; } public int Type { get; set; } public int CodeListe { get; set; } public int CodeProperty { get; set; } public string Name { get; set; } = string.Empty; }
        public enum GroupLevel { Property, Site }
    }

    public static class Common { public const string SP_GET_KEYWORDCODENAME = "GET_KEYWORDCODENAME"; }
}
