using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrefixController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SavePrefix([FromBody] Models.PrefixData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            try
            {
                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing)
                {
                    if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                }
                var codeSiteList = Common.Join(arrSharing, "(", ")", ",");

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[UPDATE_Prefix]";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
                cmd.Parameters.Add("@IsFemale", SqlDbType.Bit).Value = data.Info.Gender == "Feminine" ? 1 : 0;
                cmd.Parameters.Add("@TranslationCode", SqlDbType.Int).Value = data.Info.TranslationCode;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.IsGlobal;
                cmd.Parameters.Add("@CodeSites", SqlDbType.NVarChar, 2000).Value = codeSiteList;
                cmd.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add("@Code", SqlDbType.Int).Direction = ParameterDirection.InputOutput;

                cn.Open();
                _trans = cn.BeginTransaction();
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Save prefix failed"));

                var codePrefix = GetInt(cmd.Parameters["@Code"].Value, 0);
                if (codePrefix > 0)
                {
                    if (data.Sharing != null)
                    {
                        var sharingList = data.Sharing.Select(x => x.Code).Distinct().ToList();
                        var codeSharedTo = string.Join(",", sharingList);
                        cmd.CommandText = Common.SP_API_UPDATE_Sharing; // assuming constant in Common
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codePrefix;
                        cmd.Parameters.Add("@CodeOwner", SqlDbType.Int).Value = data.Info.CodeOwner;
                        cmd.Parameters.Add("@CodeSharedToList", SqlDbType.VarChar, 4000).Value = codeSharedTo;
                        cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 154;
                        cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.IsGlobal;
                        cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                        if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Save cookbook sharing failed"));
                    }
                }
                response.Code = 0; response.Message = "OK"; response.ReturnValue = codePrefix; response.Status = true;
                _trans.Commit();
                return Ok(response);
            }
            catch (Exception)
            {
                try { _trans?.Rollback(); } catch { }
                if (resultCode == 0) resultCode = 500;
                return StatusCode(500, Fail(response, resultCode, "Save category failed"));
            }
        }

        [HttpPost("update_recipe_ingredient_prefix")]
        public ActionResult<Models.ResponseCallBack> UpdateRecipeIngredientPrefix([FromBody] Models.PrefixGeneric data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE_RecipeIngredientPrefix";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = data.CodeListe;
                cmd.Parameters.Add("@CodePrefix", SqlDbType.Int).Value = data.CodePrefix;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cn.Open();
                cmd.ExecuteNonQuery();
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Update recipe ingredient prefix failed"));
                response.Code = 0; response.Message = "OK"; response.ReturnValue = string.Empty; response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                return StatusCode(500, Fail(response, resultCode, "Update recipe ingredient prefix failed."));
            }
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeletePrefix([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrProjectCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrProjectCodes.Contains(c.Code)) arrProjectCodes.Add(c.Code);
                }
                var codeProjectList = Common.Join(arrProjectCodes, string.Empty, string.Empty, ",");
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeProjectList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "PREFIX";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Delete prefix failed"));
                response.Code = 0; response.Message = "OK"; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value); response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode; response.Status = false; response.Message = "Delete prefix failed"; response.ReturnValue = string.Empty;
                return StatusCode(500, response);
            }
        }

        [HttpGet("{codesite:int?}/{name?}")]
        public ActionResult<List<Models.Prefix>> GetPrefix(int codesite, string name = "")
        {
            try
            {
                if (name == null) name = string.Empty;
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_PrefixList]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@vchName", SqlDbType.NVarChar).Value = name;
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();

                var prefixes = new List<Models.Prefix>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    prefixes.Add(new Models.Prefix
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Gender = GetStr(r["Gender"]),
                        TranslationCode = GetStr(r["TranslationCode"]),
                        PrefixLanguage = GetStr(r["PrefixLanguage"]),
                        IsGlobal = GetBool(r["IsGlobal"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var result = new List<Models.Prefix>();
                    foreach (var word in name.Split(','))
                    {
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var c in prefixes)
                        {
                            if (c.Name.ToLowerInvariant().Contains(w)) result.Add(c);
                        }
                    }
                    prefixes = result;
                }

                return Ok(prefixes);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("search")]
        public ActionResult<List<Models.Prefix>> GetPrefix2([FromBody] Models.ConfigurationcSearch data)
        {
            try
            {
                if (data.Name == null) data.Name = string.Empty;
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_PrefixList]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@vchName", SqlDbType.NVarChar).Value = data.Name;
                cmd.Parameters.Add("@CodeProperty", SqlDbType.NVarChar).Value = data.CodeProperty;
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();

                var prefixes = new List<Models.Prefix>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    prefixes.Add(new Models.Prefix
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Gender = GetStr(r["Gender"]),
                        TranslationCode = GetStr(r["TranslationCode"]),
                        PrefixLanguage = GetStr(r["PrefixLanguage"]),
                        IsGlobal = GetBool(r["IsGlobal"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var result = new List<Models.Prefix>();
                    foreach (var word in data.Name.Split(','))
                    {
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var c in prefixes)
                        {
                            if (c.Name.ToLowerInvariant().Contains(w)) result.Add(c);
                        }
                    }
                    prefixes = result;
                }

                return Ok(prefixes);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("getLanguageTranslations")]
        public ActionResult<List<Models.GenericList>> GetLanguageTranslations([FromBody] Models.Translation data)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_PrefixTranslationList]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = 1;
                cmd.Parameters.Add("@CodeSites", SqlDbType.NVarChar).Value = "en";
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();

                var translations = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    translations.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]) 
                    });
                }
                return Ok(translations);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("prefixtranslations/{codesite:int?}/{isglobal:bool?}/{codesites?}")]
        public ActionResult<List<Models.GenericList>> GetPrefixTranslationList(int codesite, bool isglobal, string codesites = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_PrefixTranslationList]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = isglobal;
                cmd.Parameters.Add("@CodeSites", SqlDbType.NVarChar).Value = codesites;
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();

                var translations = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    translations.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]) 
                    });
                }
                return Ok(translations);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codeprefix:int}")]
        public ActionResult<List<Models.TreeNode>> GetPrefixSharing(int codesite, int type, int tree, int codeprefix)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "[dbo].[API_GET_SharingPrefix]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodePrefix", SqlDbType.Int).Value = codeprefix;
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
                        children = CreateChildren(sharings, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName
                    };
                    sharingdata.Add(parent);
                }
                return Ok(sharingdata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> sharingdata, int code)
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
                        note = k.Global
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        // Helpers
        private static int GetInt(object value, int fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (int.TryParse(Convert.ToString(value), out var i)) return i; try { return Convert.ToInt32(value); } catch { return fallback; } }
        private static string GetStr(object value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        private static bool GetBool(object value) { if (value == null || value == DBNull.Value) return false; if (value is bool b) return b; if (int.TryParse(Convert.ToString(value), out var i)) return i != 0; if (bool.TryParse(Convert.ToString(value), out var bb)) return bb; return false; }
    }
}
