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
    public class SupplierController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/suppliers/{codesite:int}/{status:int}")]
        public ActionResult<List<Models.Supplier>> GetSupplierList(int codesite, int status)
        {
            var suppliers = new List<Models.Supplier>();
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_SupplierList]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;
                cn.Open();
                using var dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var s = new Models.Supplier
                        {
                            Name = GetStr(dr["NameRef"]),
                            Code = GetInt(dr["Code"]),
                            Number = GetInt(dr["number"]).ToString(CultureInfo.InvariantCulture), // keep VB behavior (int here)
                            City = GetStr(dr["city"]),
                            Country = GetStr(dr["country"]),
                            PhoneNumber = GetStr(dr["tel"]),
                            Fax = GetStr(dr["fax"]),
                            Global = GetBool(dr["IsGlobal"]) 
                        };
                        suppliers.Add(s);
                    }
                }
                dr.Close();
                return Ok(suppliers);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(400); }
        }

        [HttpGet("/api/getsupplier/{codesite:int}/{codesupplier:int?}")]
        public ActionResult<List<Models.Supplier>> GetSupplier(int codesite, int? codesupplier = -1)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[SUP_GetList]";
                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codesupplier ?? -1;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = -1;
                if ((codesupplier ?? -1) == -1)
                {
                    cmd.Parameters.Add("@Status", SqlDbType.Int).Value = -1;
                }
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var list = new List<Models.Supplier>();
                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow r in ds.Tables[1].Rows)
                    {
                        list.Add(new Models.Supplier
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["NameRef"]),
                            Global = GetBool(r["IsGlobal"]),
                            Number = GetStr(r["Number"]),
                            City = GetStr(r["City"]),
                            Country = GetStr(r["Country"]),
                            PhoneNumber = GetStr(r["Tel"]),
                            Address1 = GetStr(r["Address1"]),
                            Address2 = GetStr(r["Address2"]),
                            Company = GetStr(r["Company"]),
                            ZipCode = GetStr(r["Zip"]),
                            State = GetStr(r["State"]),
                            Fax = GetStr(r["Fax"]),
                            Email = GetStr(r["Email"]),
                            URL = GetStr(r["URL"]),
                            Remark = GetStr(r["Remark"]),
                            Note = GetStr(r["Note"]) 
                        });
                    }
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/supplier/{codesite:int}/{type:int}/{name?}")]
        public ActionResult<List<Models.Supplier>> GetSupplierByName(int codesite, int type, string name = "")
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSUPPLIER";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var list = new List<Models.Supplier>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Supplier
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]),
                        Number = GetInt(r["Number"]).ToString(CultureInfo.InvariantCulture),
                        City = GetStr(r["City"]),
                        Country = GetStr(r["Country"]),
                        PhoneNumber = GetStr(r["PhoneNumber"]),
                        Fax = GetStr(r["Fax"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var results = new List<Models.Supplier>();
                    foreach (var w in name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in list)
                        {
                            if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                        }
                    }
                    list = results;
                }

                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("/api/supplier/search")]
        public ActionResult<List<Models.Supplier>> GetSupplierByName2([FromBody] Models.ConfigurationcSearch data)
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSUPPLIER";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var list = new List<Models.Supplier>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Supplier
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]),
                        Number = GetStr(r["Number"]),
                        City = GetStr(r["City"]),
                        Country = GetStr(r["Country"]),
                        PhoneNumber = GetStr(r["PhoneNumber"]),
                        Fax = GetStr(r["Fax"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var results = new List<Models.Supplier>();
                    foreach (var w in data.Name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in list)
                        {
                            if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                        }
                    }
                    list = results;
                }

                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/supplier/sharing/{codesite:int}/{type:int}/{tree:int}/{codesupplier:int}")]
        public ActionResult<List<Models.TreeNode>> GetSupplierSharing(int codesite, int type, int tree, int codesupplier)
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
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codesupplier;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 120;
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

        [HttpPost("api/supplier")]
        public ActionResult<Models.ResponseCallBack> SaveSupplier([FromBody] Models.SupplierData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction trans = null;
            try
            {
                using var cmd = new SqlCommand();
                var arrSharing = new ArrayList();
                var arrMerge = new ArrayList();

                if (data?.Sharing != null)
                {
                    foreach (var sh in data.Sharing)
                    {
                        if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                    }
                }
                if (data?.MergeList != null)
                {
                    foreach (var ml in data.MergeList)
                    {
                        if (!arrMerge.Contains(ml)) arrMerge.Add(ml);
                    }
                }

                var codeSiteList = Common.Join(arrSharing, "(", ")", ",");
                var codeMergeList = Common.Join(arrMerge, "(", ")", ",");

                int tran;
                if (data.Info.Code == -1) tran = 1;
                else if (data.Info.Code == -2) tran = 4;
                else tran = 2;
                if (data.Info.ActionType == 5) tran = 4;

                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[SUP_Update]";
                cmd.Parameters.Clear();

                var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Info.CodeUser;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Info.CodeSite;
                var pCode = cmd.Parameters.Add("@intCode", SqlDbType.Int);
                pCode.Value = data.Info.Code;

                cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 15).Value = data.Info.Number ?? string.Empty;
                cmd.Parameters.Add("@nvcNameRef", SqlDbType.NVarChar, 15).Value = data.Info.Name ?? string.Empty;
                cmd.Parameters.Add("@nvcCompany", SqlDbType.NVarChar, 50).Value = data.Info.Company ?? string.Empty;
                cmd.Parameters.Add("@nvcURL", SqlDbType.NVarChar, 50).Value = data.Info.URL ?? string.Empty;
                cmd.Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = data.Info.Note ?? string.Empty;
                cmd.Parameters.Add("@nvcTerms", SqlDbType.NVarChar, 2000).Value = string.Empty;
                cmd.Parameters.Add("@UseDefaultTerms", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@nvcAcctRef", SqlDbType.NVarChar, 20).Value = string.Empty;
                cmd.Parameters.Add("@nvcAddress1", SqlDbType.NVarChar, 200).Value = data.Info.Address1 ?? string.Empty;
                cmd.Parameters.Add("@nvcAddress2", SqlDbType.NVarChar, 200).Value = data.Info.Address2 ?? string.Empty;
                cmd.Parameters.Add("@WithTax", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@nvcCity", SqlDbType.NVarChar, 30).Value = data.Info.City ?? string.Empty;
                cmd.Parameters.Add("@nvcZip", SqlDbType.NVarChar, 15).Value = data.Info.ZipCode ?? string.Empty;
                cmd.Parameters.Add("@nvcCountry", SqlDbType.NVarChar, 30).Value = data.Info.Country ?? string.Empty;
                cmd.Parameters.Add("@nvcState", SqlDbType.NVarChar, 30).Value = data.Info.State ?? string.Empty;
                cmd.Parameters.Add("@nvcTel", SqlDbType.NVarChar, 15).Value = data.Info.PhoneNumber ?? string.Empty;
                cmd.Parameters.Add("@nvcFax", SqlDbType.NVarChar, 15).Value = data.Info.Fax ?? string.Empty;
                cmd.Parameters.Add("@nvcEmail", SqlDbType.NVarChar, 50).Value = data.Info.Email ?? string.Empty;
                cmd.Parameters.Add("@nvcCity2", SqlDbType.NVarChar, 30).Value = string.Empty;
                cmd.Parameters.Add("@nvcZip2", SqlDbType.NVarChar, 15).Value = string.Empty;
                cmd.Parameters.Add("@nvcCountry2", SqlDbType.NVarChar, 30).Value = string.Empty;
                cmd.Parameters.Add("@nvcState2", SqlDbType.NVarChar, 30).Value = string.Empty;
                cmd.Parameters.Add("@nvcRemark", SqlDbType.NVarChar, 30).Value = data.Info.Remark ?? string.Empty;
                cmd.Parameters.Add("@intCodeSupplierGroup", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@AddFlag", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@UpdateFlag", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@ImportFlag", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = tran;

                pCode.Direction = ParameterDirection.InputOutput;

                codeSiteList = (codeSiteList ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(codeSiteList))
                {
                    if (codeSiteList.StartsWith("(") && codeSiteList.EndsWith(")"))
                    {
                        cmd.Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = codeSiteList;
                    }
                    else
                    {
                        throw new DatabaseException($"[{resultCode}] Save supplier failed");
                    }
                }

                if (tran == 4)
                {
                    codeMergeList = (codeMergeList ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(codeMergeList))
                    {
                        if (codeMergeList.StartsWith("(") && codeMergeList.EndsWith(")"))
                        {
                            cmd.Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = codeMergeList;
                        }
                        else
                        {
                            throw new DatabaseException($"[{resultCode}] Save supplier failed");
                        }
                    }
                }

                retval.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
                var codeKiosk = GetInt(pCode.Value, -1);
                resultCode = GetInt(retval.Value, -1);
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save supplier failed");

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = codeKiosk;
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
                response.Message = "Save kiosk failed"; // mirror VB behavior
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

        [HttpPost("api/supplier/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteSupplier([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
                var joined = codes.Count > 0 ? $"({string.Join(",", codes)})" : string.Empty;

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "SUP_Delete";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = 0;
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = 7;
                var skip = cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000);
                skip.Direction = ParameterDirection.Output;

                joined = (joined ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(joined))
                {
                    if (joined.StartsWith("(") && joined.EndsWith(")"))
                    {
                        cmd.Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = joined;
                    }
                    else
                    {
                        throw new DatabaseException($"[{resultCode}] Delete supplier failed");
                    }
                }

                var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                retval.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(retval.Value, -1);
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete supplier failed");

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
                response.Message = "Delete supplier failed";
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
