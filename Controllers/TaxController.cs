using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class TaxController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/tax/{codesite:int}")]
        public ActionResult<List<Models.Tax>> GetTax(int codesite)
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWTAX";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var taxes = new List<Models.Tax>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    taxes.Add(new Models.Tax
                    {
                        TaxCode = GetInt(r["Code"]),
                        TaxValue = GetDbl(r["Value"]),
                        TaxName = GetStr(r["Description"]),
                        Global = GetBool(r["Global"]) 
                    });
                }
                return Ok(taxes);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/tax/{codesite:int}/{type:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.Tax>> GetTaxByName(int codesite, int type, int? codeproperty = -1, string name = "")
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
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWTAX";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty ?? -1;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var taxes = new List<Models.Tax>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    taxes.Add(new Models.Tax
                    {
                        TaxCode = GetInt(r["Code"]),
                        TaxValue = GetDbl(r["Value"]),
                        TaxName = GetStr(r["Description"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var results = new List<Models.Tax>();
                    foreach (var w in name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in taxes)
                        {
                            if ((s.TaxValue.ToString(CultureInfo.InvariantCulture).ToLowerInvariant().Contains(key)) ||
                                (!string.IsNullOrEmpty(s.TaxName) && s.TaxName.ToLowerInvariant().Contains(key)))
                            {
                                results.Add(s);
                            }
                        }
                    }
                    taxes = results;
                }

                return Ok(taxes);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("/api/tax/search")]
        public ActionResult<List<Models.Tax>> GetTaxByName2([FromBody] Models.ConfigurationcSearch data)
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
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWTAX";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var taxes = new List<Models.Tax>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    taxes.Add(new Models.Tax
                    {
                        TaxCode = GetInt(r["Code"]),
                        TaxValue = GetDbl(r["Value"]),
                        TaxName = GetStr(r["Description"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var results = new List<Models.Tax>();
                    foreach (var w in data.Name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in taxes)
                        {
                            if ((s.TaxValue.ToString(CultureInfo.InvariantCulture).ToLowerInvariant().Contains(key)) ||
                                (!string.IsNullOrEmpty(s.TaxName) && s.TaxName.ToLowerInvariant().Contains(key)))
                            {
                                results.Add(s);
                            }
                        }
                    }
                    taxes = results;
                }

                return Ok(taxes);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/GetTax/{codesite:int}/{codetax:int}")]
        public ActionResult<List<Models.Tax>> GetTaxList(int codesite, int codetax)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_TAXCODEVALUEDESC]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = true;
                cmd.Parameters.Add("@CodeTax", SqlDbType.Int).Value = codetax;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var taxes = new List<Models.Tax>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    taxes.Add(new Models.Tax
                    {
                        TaxCode = GetInt(r["Code"]),
                        TaxValue = GetDbl(r["Value"]),
                        TaxName = GetStr(r["Description"]) 
                    });
                }
                return Ok(taxes);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/tax/sharing/{codesite:int}/{type:int}/{tree:int}/{codetax:int}")]
        public ActionResult<List<Models.TreeNode>> GetTaxSharing(int codesite, int type, int tree, int codetax)
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
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codetax;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 126;
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

        [HttpPost("api/tax")]
        public ActionResult<Models.ResponseCallBack> SaveTax([FromBody] Models.TaxData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
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
                var codeSiteList = Common.Join(arrSharing, "(", ")", ",");

                var tran = data.Info.TaxCode == -1 ? 1 : (data.Info.TaxCode == -2 ? 4 : 2);

                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_EgswTaxUpdate";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                var pCode = cmd.Parameters.Add("@intCode", SqlDbType.Int);
                pCode.Value = data.Info.TaxCode;
                cmd.Parameters.Add("@nvcDesc", SqlDbType.NVarChar, 50).Value = data.Info.TaxName ?? string.Empty;
                cmd.Parameters.Add("@fltValue", SqlDbType.Float).Value = data.Info.TaxValue;
                cmd.Parameters.Add("@nvcNumberRef", SqlDbType.NVarChar, 20).Value = string.Empty;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = tran;
                var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                cmd.Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = codeSiteList ?? string.Empty;

                // Merge support
                if (tran == 4 && data.MergeList != null && data.MergeList.Count > 0)
                {
                    var arrMerge = new ArrayList();
                    foreach (var m in data.MergeList) if (!arrMerge.Contains(m)) arrMerge.Add(m);
                    var mergeList = Common.Join(arrMerge, "(", ")", ",");
                    cmd.Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = mergeList ?? string.Empty;
                }

                pCode.Direction = ParameterDirection.InputOutput;
                retval.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(retval.Value, -1);
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save tax failed");

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = GetInt(pCode.Value, -1);
                response.Status = true;
                return Ok(response);
            }
            catch (DatabaseException)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save tax failed";
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

        [HttpPost("api/tax/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteTax([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
                var joined = string.Join(",", codes); // VB passed without parentheses

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = joined;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWTAX";
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
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete tax failed");

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
                response.Message = "Delete tax failed";
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
                        selected = k.Flagged,
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
