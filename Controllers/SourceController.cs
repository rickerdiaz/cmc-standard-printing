using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class SourceController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/source/{codesite:int}")]
        public ActionResult<List<Models.GenericCodeValueList>> GetSource(int codesite)
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
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSOURCE";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var list = new List<Models.GenericCodeValueList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.GenericCodeValueList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("/api/source/search")]
        public ActionResult<List<Models.Source>> GetSourceByName2([FromBody] Models.ConfigurationcSearch data)
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSOURCE";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var sources = new List<Models.Source>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sources.Add(new Models.Source
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var results = new List<Models.Source>();
                    foreach (var w in data.Name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in sources)
                        {
                            if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                        }
                    }
                    sources = results;
                }

                return Ok(sources);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/source/{codesite:int}/{type:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.Source>> GetSourceByName(int codesite, int type, int? codeproperty = -1, string name = "")
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSOURCE";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty ?? -1;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var sources = new List<Models.Source>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sources.Add(new Models.Source
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var results = new List<Models.Source>();
                    foreach (var w in name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in sources)
                        {
                            if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                        }
                    }
                    sources = results;
                }

                return Ok(sources);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/getsource/{codesite:int}/{codesource:int?}")]
        public ActionResult<List<Models.Source>> GetSourceList(int codesite, int? codesource = -1)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_SOURCECODENAME]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = true;
                cmd.Parameters.Add("@CodeSource", SqlDbType.Int).Value = codesource ?? -1;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var sources = new List<Models.Source>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sources.Add(new Models.Source
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }
                return Ok(sources);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/source/sharing/{codesite:int}/{type:int}/{tree:int}/{codesource:int}")]
        public ActionResult<List<Models.TreeNode>> GetSourceSharing(int codesite, int type, int tree, int codesource)
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
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codesource;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 117;
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

                var result = new List<Models.TreeNode>();
                var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
                foreach (var p in parents)
                {
                    if (result.All(o => o.key != p.Code))
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
                            // groupLevel = GroupLevel.Property
                        };
                        if (parent.children != null && parent.children.Count > 0)
                        {
                            result.Add(parent);
                        }
                    }
                }

                return Ok(result);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/source")]
        public ActionResult<Models.ResponseCallBack> SaveSource([FromBody] Models.SourceData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction trans = null;
            var strCodesToMerge = string.Empty;

            try
            {
                if (data?.MergeList != null && data.MergeList.Count > 0)
                {
                    foreach (var s in data.MergeList)
                    {
                        strCodesToMerge = string.IsNullOrEmpty(strCodesToMerge) ? s.ToString() : strCodesToMerge + "," + s.ToString();
                    }
                }

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);

                var arrSharing = new ArrayList();
                if (data?.Sharing != null)
                {
                    foreach (var sh in data.Sharing)
                    {
                        if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                    }
                }
                var codeSiteList = Common.Join(arrSharing, "(", ")", ",");
                var tran = (data.Info.Code == -1 || data.Info.Code == -2) ? 1 : 2;

                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[sp_EgswSourceUpdate]";
                cmd.Parameters.Clear();
                var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 50).Value = data.Info.Name ?? string.Empty;
                cmd.Parameters.Add("@EGSID", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = tran;
                cmd.Parameters.Add("@vchCodeSiteList", SqlDbType.NVarChar, 8000).Value = codeSiteList ?? string.Empty;
                cmd.Parameters.Add("@CodesToMerge", SqlDbType.NVarChar, 2000).Value = strCodesToMerge ?? string.Empty;
                cmd.Parameters["@intCode"].Direction = ParameterDirection.InputOutput;
                retval.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();

                var codeSource = GetInt(cmd.Parameters["@intCode"].Value, -1);
                resultCode = GetInt(retval.Value, -1);
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save source failed");

                if (data.ActionType == 5 && data.MergeList != null && data.MergeList.Count > 0)
                {
                    var arrSites = new ArrayList();
                    foreach (var s in data.MergeList) if (!arrSites.Contains(s)) arrSites.Add(s);
                    var siteList = Common.Join(arrSites, "(", ")", ",");

                    // Insert sharing rows
                    var sql = new StringBuilder();
                    sql.Append("INSERT INTO EgswSharing ");
                    sql.Append("SELECT @Code,0,CodeUserSharedTo,1,117,0,1,0 ");
                    sql.Append("FROM EgswSharing ");
                    sql.Append("WHERE Code=@Code AND CodeEgswTable=117 AND [Status]=1 AND [Type] IN (1,5) ");
                    sql.Append("    AND CodeUserSharedTo NOT IN (");
                    sql.Append("    SELECT DISTINCT CodeUserSharedTo ");
                    sql.Append("    FROM EgswSharing  ");
                    sql.Append($"    WHERE Code IN {siteList} AND CodeEgswTable=117 AND [Status]=1 AND [Type] IN (1,5)");
                    sql.Append("    ) ");
                    cmd.CommandText = sql.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codeSource;
                    cmd.ExecuteNonQuery();

                    // Update liste source
                    var mergeList = Common.Join(new ArrayList(data.MergeList), "(", ")", ",");
                    sql.Clear();
                    sql.Append("UPDATE EgswListe ");
                    sql.Append("SET Source=@newSoureeCode ");
                    sql.Append($" WHERE Source IN {mergeList}");
                    cmd.CommandText = sql.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@newSoureeCode", SqlDbType.Int).Value = codeSource;
                    cmd.ExecuteNonQuery();

                    // Delete merged items
                    cmd.CommandText = "API_DELETE_Generic";
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var code in arrSites)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = GetInt(code);
                        cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWSOURCE";
                        cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                        cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = false;
                        cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                        var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                        ret.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(ret.Value, -1);
                        if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete merged category failed");
                    }
                }

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = codeSource;
                response.Status = true;
                trans.Commit();
                return Ok(response);
            }
            catch (DatabaseException)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save source failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        [HttpPost("api/source/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteSource([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
                var joined = string.Join(",", codes);

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = joined;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWSOURCE";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                var skip = cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000);
                skip.Direction = ParameterDirection.Output;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete source failed");

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = GetStr(skip.Value);
                response.Status = true;
                return Ok(response);
            }
            catch (DatabaseException)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.ReturnValue = string.Empty;
                response.Status = false;
                response.Message = "Delete source failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        [HttpPost("api/source/purge/{type:int}/{codeUser:int}/{codeSite:int}")]
        public ActionResult<Models.ResponseCallBack> PurgeSource(int type, long codeUser, long codeSite)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_PURGE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWSOURCE";
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0)
                {
                    if (resultCode == -480)
                    {
                        response.Code = -480;
                        response.Message = "Nothing was deleted";
                        response.Status = false;
                        return Ok(response);
                    }
                    else
                    {
                        throw new DatabaseException($"[{resultCode}] Purge source failed");
                    }
                }

                response.Code = 0;
                response.Message = "OK";
                response.Status = true;
                return Ok(response);
            }
            catch (DatabaseException)
            {
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Purge source failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Helpers
        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> sharingdata, int code)
        {
            var children = new List<Models.TreeNode>();
            if (sharingdata != null)
            {
                var kids = sharingdata.Where(o => o.ParentCode == code && o.Type == 2).OrderBy(o => o.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.TreeNode
                    {
                        title = k.Name,
                        key = k.Code,
                        icon = false,
                        children = null,
                        select = k.Flagged,
                        parenttitle = k.ParentName,
                        note = k.Global
                    };
                    children.Add(child);
                }
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
        private static double GetDbl(object value, double fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (value is double d) return d;
            if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var dd)) return dd;
            try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); } catch { return fallback; }
        }
        private static string GetStr(object value, string fallback = "")
        {
            if (value == null || value == DBNull.Value) return fallback;
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        private static bool GetBool(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            if (value is bool b) return b;
            if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
            if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
            return false;
        }
    }
}
