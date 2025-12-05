using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class SaleSiteController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpPost("/api/salesite/search")]
        public ActionResult<List<Models.SaleSite>> GetSaleSiteByName2([FromBody] Models.ConfigurationcSearch data)
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
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSALESITE";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var salesite = new List<Models.SaleSite>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    salesite.Add(new Models.SaleSite
                    {
                        Code = GetInt(r["Code"]),
                        LocationNumber = GetStr(r["LocationNumber"]),
                        Name = GetStr(r["Name"]),
                        Street = GetStr(r["Street"]),
                        ZipCode = GetStr(r["ZipCode"]),
                        City = GetStr(r["City"]),
                        CertificationID = GetStr(r["CertificationID"]),
                        isProductionLocation = GetStr(r["isProductionLocation"]),
                        isSalesSite = GetStr(r["isSalesSite"]),
                        codeLanguage = GetStr(r["codeLanguage"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var results = new List<Models.SaleSite>();
                    var arrNames = data.Name.Split(',');
                    foreach (var w in arrNames)
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in salesite)
                        {
                            if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key))
                            {
                                results.Add(s);
                            }
                        }
                    }
                    salesite = results;
                }

                return Ok(salesite);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("/api/salesite/sharing/{codesite:int}/{type:int}/{tree:int}/{codesalesite:int}")]
        public ActionResult<List<Models.TreeNode>> GetsalesiteSharing(int codesite, int type, int tree, int codesalesite)
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
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codesalesite;
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

                var sharingdata = new List<Models.TreeNode>();
                var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
                foreach (var p in parents)
                {
                    if (sharingdata.All(o => o.key != p.Code))
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
                            // groupLevel = GroupLevel.Property // keep as default if enum not referenced here
                        };
                        if (parent.children != null && parent.children.Count > 0)
                        {
                            sharingdata.Add(parent);
                        }
                    }
                }

                return Ok(sharingdata);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("api/salesite")]
        public ActionResult<Models.ResponseCallBack> SaveSaleSite([FromBody] Models.SaleSiteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction trans = null;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "MANAGE_SALESITEUPDATE";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@LocationNumber", SqlDbType.NVarChar).Value = data.Info.LocationNumber;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = data.Info.Name;
                cmd.Parameters.Add("@Street", SqlDbType.NVarChar, 50).Value = data.Info.Street;
                cmd.Parameters.Add("@ZipCode", SqlDbType.Int).Value = data.Info.ZipCode;
                cmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = data.Info.City;
                cmd.Parameters.Add("@CertificationID", SqlDbType.NVarChar).Value = data.Info.CertificationID;
                cmd.Parameters.Add("@IsProductionLocation", SqlDbType.Bit).Value = data.Info.isProductionLocation;
                cmd.Parameters.Add("@IsSalesSite", SqlDbType.Bit).Value = data.Info.isSalesSite;
                cmd.Parameters.Add("@Codelanguage", SqlDbType.Int).Value = data.Info.codeLanguage;

                var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                retval.Direction = ParameterDirection.ReturnValue;

                // Optional sharing params
                var arrSharing = new ArrayList();
                var codeSiteList = Common.Join(arrSharing, "(", ")", ",");
                codeSiteList = (codeSiteList ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(codeSiteList))
                {
                    if (codeSiteList.StartsWith("(") && codeSiteList.EndsWith(")"))
                    {
                        cmd.Parameters.Add("@vchCodeSiteList", SqlDbType.Text).Value = codeSiteList;
                        cmd.Parameters.Add("@vchCodeSiteList2", SqlDbType.Text).Value = codeSiteList.Replace("(", string.Empty).Replace(")", string.Empty);
                    }
                    else
                    {
                        resultCode = -1; // invalid format
                    }
                }

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.CommandTimeout = 120;
                cmd.ExecuteNonQuery();

                var codeSaleSite = GetInt(cmd.Parameters["@Code"].Value, -1);
                resultCode = GetInt(retval.Value, -1);
                if (resultCode != 0)
                {
                    throw new DatabaseException($"[{resultCode}] Save Sale Site failed");
                }

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = codeSaleSite;
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
                response.Message = "Save Sale Site failed";
                return StatusCode(500, response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        [HttpGet("/api/salesite/getsalesite/{code:int}/{codesite:int}")]
        public ActionResult<List<Models.SaleSite>> GetSaleSite(int code, int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_SaleSite]";
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var salessite = new List<Models.SaleSite>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    salessite.Add(new Models.SaleSite
                    {
                        Code = GetInt(r["Code"]),
                        LocationNumber = GetStr(r["LocationNumber"]),
                        Name = GetStr(r["Name"]),
                        Street = GetStr(r["Street"]),
                        ZipCode = GetStr(r["ZipCode"]),
                        City = GetStr(r["City"]),
                        CertificationID = GetStr(r["CertificationID"]),
                        isProductionLocation = GetBool(r["isProductionLocation"]),
                        isSalesSite = GetBool(r["isSalesSite"]),
                        codeLanguage = GetStr(r["codeLanguage"]) 
                    });
                }

                return Ok(salessite);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("api/salesite/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteSaleSite([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            var resultCode = 0;
            try
            {
                var codeList = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
                var joined = string.Join(",", codeList);

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = joined;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWSALESSITE";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                var skip = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
                skip.Direction = ParameterDirection.Output;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;

                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0)
                {
                    throw new DatabaseException($"[{resultCode}] Delete sales site failed");
                }

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
                response.Status = false;
                response.ReturnValue = string.Empty;
                response.Message = "Delete sales site failed";
                return StatusCode(500, response);
            }
            catch (ArgumentException)
            {
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                return BadRequest(response);
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Helpers (copied from C# RecipeController for consistency)
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

        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> nodes, int code)
        {
            var children = new List<Models.TreeNode>();
            if (nodes == null) return children;
            var kids = nodes.Where(o => o.Code != code && o.ParentCode == code && code > 0).OrderBy(o => o.Name).ToList();
            foreach (var k in kids)
            {
                var child = new Models.TreeNode
                {
                    title = k.Name,
                    key = k.Code,
                    icon = false,
                    children = CreateChildrenSharing(nodes, k.Code),
                    select = k.Flagged,
                    selected = k.Flagged,
                    parenttitle = k.ParentName
                };
                children.Add(child);
            }
            return children;
        }
    }
}
