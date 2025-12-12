using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class TimeController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/paulo/")]
        public ActionResult<List<Models.Time>> GetPaulo()
        {
            var list = new List<Models.Time>
            {
                new Models.Time { Name = "ERIKA ARANAS", Code = 123, Global = true },
                new Models.Time { Name = "LEIKA ARANAS", Code = 124, Global = false }
            };
            return Ok(list);
        }

        [HttpGet("/api/times/{codesite:int}/{codetrans:int}/{name?}")]
        public ActionResult<List<Models.Time>> GetTimes(int codesite, int codetrans, string name = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "sp_GetTimebySite";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int, 4).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int, 4).Value = codetrans;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var times = new List<Models.Time>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    times.Add(new Models.Time
                    {
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["IsGlobal"]),
                        Code = GetInt(r["Code"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var result = new List<Models.Time>();
                    foreach (var w in name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var t in times)
                        {
                            if (!string.IsNullOrEmpty(t.Name) && t.Name.ToLowerInvariant().Contains(key)) result.Add(t);
                        }
                    }
                    times = result;
                }

                return Ok(times);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("/api/times/search")]
        public ActionResult<List<Models.Time>> GetTimes2([FromBody] Models.ConfigurationcSearch data2)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "sp_GetTimebySite";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int, 4).Value = data2.CodeSite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int, 4).Value = data2.CodeTrans;
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int, 4).Value = data2.CodeProperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var times = new List<Models.Time>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    times.Add(new Models.Time
                    {
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["IsGlobal"]),
                        Code = GetInt(r["Code"]),
                        isTotal = GetBool(r["isTotal"]),
                        RequiredTotal = GetBool(r["RequiredTotal"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data2.Name))
                {
                    var result = new List<Models.Time>();
                    foreach (var w in data2.Name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var t in times)
                        {
                            if (!string.IsNullOrEmpty(t.Name) && t.Name.ToLowerInvariant().Contains(key)) result.Add(t);
                        }
                    }
                    times = result;
                }

                return Ok(times);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/time/{codetime:int}")]
        public ActionResult<Models.TimeData> GetTime(int codetime)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_GetTime";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@codeTime", SqlDbType.Int, 4).Value = codetime;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var td = new Models.TimeData();
                td.Sites = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    if (GetBool(r["IsSiteUsed"]))
                    {
                        td.Sites.Add(new Models.GenericTree
                        {
                            Name = GetStr(r["SiteName"]),
                            ParentCode = GetInt(r["CodeProperty"]),
                            Code = GetInt(r["CodeSite"]) 
                        });
                    }
                }
                td.Translation = new List<Models.GenericTranslation>();
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    td.Translation.Add(new Models.GenericTranslation
                    {
                        Code = GetInt(r["CodeTrans"]),
                        Name = GetStr(r["Name"]) 
                    });
                }
                td.Info = new Models.Time();
                foreach (DataRow r in ds.Tables[2].Rows)
                {
                    td.Info.Name = GetStr(r["Name"]);
                    td.Info.Global = GetBool(r["IsGlobal"]);
                }

                return Ok(td);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/times/sharing/{codesite:int}/{type:int}/{tree:int}/{codetime:int}")]
        public ActionResult<List<Models.TreeNode>> GetTimeSharing(int codesite, int type, int tree, int codetime)
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
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codetime;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 153;
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
                            children = CreateChildren(sharings, p.Code),
                            select = p.Flagged,
                            parenttitle = p.ParentName
                        };
                        result.Add(parent);
                    }
                }

                return Ok(result);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/time/translation/{codetime:int}/{codesite:int}")]
        public ActionResult<List<Models.RecipeTranslation>> GetTimeTranslation(int codetime, int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_TimeTranslation]";
                cmd.Parameters.Add("@CodeTime", SqlDbType.Int).Value = codetime;
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
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/time")]
        public ActionResult<Models.ResponseCallBack> SaveTime([FromBody] Models.TimeData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction trans = null;
            try
            {
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
                // VB uses no parentheses for Join here
                var codeSiteList = Common.Join(arrSharing, string.Empty, string.Empty, ",");

                var tranMode = (data.Info.Code == -1 || data.Info.Code == -2) ? 1 : 2;

                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[sp_UpdateTime]";
                cmd.Parameters.Clear();
                var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                var pCode = cmd.Parameters.Add("@Code", SqlDbType.Int);
                pCode.Value = data.Info.Code;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = data.Info.Name ?? string.Empty;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 100).Value = codeSiteList ?? string.Empty;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.Info.CodeTrans;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                cmd.Parameters.Add("@IsTotal", SqlDbType.Bit).Value = data.Info.isTotal;
                cmd.Parameters.Add("@ReqIsTotal", SqlDbType.Bit).Value = data.Info.RequiredTotal;

                pCode.Direction = ParameterDirection.InputOutput;
                retval.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();

                var codetime = GetInt(pCode.Value, -1);
                resultCode = GetInt(retval.Value, -1);
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save time failed");

                if (codetime > 0 && data.Translation != null)
                {
                    cmd.CommandText = "sp_UpdateTimeTranslation";
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var t in data.Translation)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@intTimeCode", SqlDbType.Int, 4).Value = codetime;
                        cmd.Parameters.Add("@vchTimeTypeName", SqlDbType.NVarChar, 150).Value = t.Name ?? string.Empty;
                        cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int, 4).Value = t.CodeTrans;
                        var ret = cmd.Parameters.Add("@intRetCode", SqlDbType.Int, 4);
                        ret.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(ret.Value, 0);
                        if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save time failed");
                    }
                }

                if (codetime != -1)
                {
                    // delete existing sharing rows
                    cmd.CommandText = $"DELETE FROM EgswSharing WHERE Code={codetime} AND CodeUserOwner={data.Profile.CodeSite} AND CodeEgswTable=153 AND Type=1";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();

                    // insert sharing rows
                    cmd.CommandText = "sp_EgswUpdateSharing";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCode", SqlDbType.Int);
                    cmd.Parameters.Add("@intCodeSite", SqlDbType.Int);
                    cmd.Parameters.Add("@intCodeSitesShared", SqlDbType.Int);
                    cmd.Parameters.Add("@intCodeEgswTable", SqlDbType.Int);
                    cmd.Parameters.Add("@isGlobal", SqlDbType.Bit);

                    string[] arrCodeSites;
                    if (!string.Equals(codeSiteList, "-1", StringComparison.Ordinal))
                    {
                        arrCodeSites = (codeSiteList ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var s in arrCodeSites)
                        {
                            if (int.TryParse(s, out var siteCode))
                            {
                                cmd.Parameters["@intCode"].Value = codetime;
                                cmd.Parameters["@intCodeSite"].Value = data.Profile.CodeSite;
                                cmd.Parameters["@intCodeSitesShared"].Value = siteCode;
                                cmd.Parameters["@intCodeEgswTable"].Value = 153;
                                cmd.Parameters["@isGlobal"].Value = data.Info.Global;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        cmd.Parameters["@intCode"].Value = codetime;
                        cmd.Parameters["@intCodeSite"].Value = data.Profile.CodeSite;
                        cmd.Parameters["@intCodeSitesShared"].Value = -1;
                        cmd.Parameters["@intCodeEgswTable"].Value = 153;
                        cmd.Parameters["@isGlobal"].Value = data.Info.Global;
                        cmd.ExecuteNonQuery();
                    }
                }

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = codetime;
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
                response.Message = "Save time failed";
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

        [HttpPost("api/time/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteTime([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
                // VB passes codes joined by comma without parentheses for Time
                var joined = string.Join(",", codes);

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = joined;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "TIMETYPE";
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
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete time failed");

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
                response.Message = "Delete time failed";
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

        [HttpGet("/api/time/getRequiredTotal")]
        public ActionResult<List<Models.Time>> GetTimesRequiredTotal()
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "SELECT RequiredTotal from TimeType";
                cmd.CommandType = CommandType.Text;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var list = new List<Models.Time>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Time { RequiredTotal = GetBool(r["RequiredTotal"]) });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers
        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> sharingdata, int code)
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
