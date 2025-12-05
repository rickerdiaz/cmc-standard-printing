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
    public class SetPriceController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/setprice/sharing/{codesite:int}/{type:int}/{tree:int}/{codesetprice:int}")]
        public ActionResult<List<Models.TreeNode>> GetSetPriceSharing(int codesite, int type, int tree, int codesetprice)
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
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 2; // match VB behavior
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codesetprice;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 110;
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
                        var parentNode = new Models.TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateChildren(sharings, p.Code),
                            select = p.Flagged,
                            parenttitle = p.ParentName
                        };
                        sharingdata.Add(parentNode);
                    }
                }

                return Ok(sharingdata);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/getsetprice/{codesite:int}/{codesetprice?}")]
        public ActionResult<List<Models.SetPrice>> GetSetPriceList(int codesite, int? codesetprice = null)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[sp_EgswSetPriceGetList]";
                var codeVal = codesetprice ?? -1;
                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codeVal;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = 1;
                cmd.Parameters.Add("@tntType", SqlDbType.Int).Value = (codeVal == -1 ? -1 : 1);
                cmd.Parameters.Add("@bitGlobalOnly", SqlDbType.Bit).Value = 0;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var list = new List<Models.SetPrice>();
                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow r in ds.Tables[1].Rows)
                    {
                        list.Add(new Models.SetPrice
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["Name"]),
                            Main = GetBool(r["Main"]),
                            hasMain = GetInt(r["hasMain"]),
                            Global = GetBool(r["IsGlobal"]),
                            CodeCurrency = GetInt(r["CodeCurrency"]),
                            Format = GetStr(r["Format"]),
                            Symbole = GetStr(r["Symbole"]),
                            Description = GetStr(r["Description"]),
                            chkDisable = GetBool(r["chkDisable"]),
                            Factor = GetDbl(r["FactorToMain"]) 
                        });
                    }
                }

                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/setprice/{code:int}")]
        public ActionResult<DataTable> GetSetPriceByCodeSource(int code)
        {
            try
            {
                var dt = new DataTable();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_SetPriceBySource]";
                cmd.Parameters.Add("@codeSource", SqlDbType.Int).Value = code;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return Ok(dt);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/setprice/{codesite:int}/{type:int}/{name?}")]
        public ActionResult<List<Models.SetPrice>> GetSetPriceByName(int codesite, int type, string name = "")
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSETPRICE";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var setprice = new List<Models.SetPrice>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    setprice.Add(new Models.SetPrice
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var results = new List<Models.SetPrice>();
                    foreach (var w in name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in setprice)
                        {
                            if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                        }
                    }
                    setprice = results;
                }

                return Ok(setprice);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/setprice/search")]
        public ActionResult<List<Models.SetPrice>> GetSetPriceByName2([FromBody] Models.ConfigurationcSearch data)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWSETPRICE";
                cmd.CommandTimeout = 300;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var setprice = new List<Models.SetPrice>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    setprice.Add(new Models.SetPrice
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Factor = GetStr(r["Factor"]),
                        FactorToMain = GetDbl(r["FactorToMain"]),
                        Main = GetBool(r["Main"]),
                        hasMain = GetInt(r["hasMain"]),
                        Global = GetBool(r["Global"]),
                        isUserDefault = GetBool(r["IsUserDefault"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var results = new List<Models.SetPrice>();
                    foreach (var w in data.Name.Split(','))
                    {
                        var word = (w ?? string.Empty).Trim();
                        if (word.Length == 0) continue;
                        var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                        foreach (var s in setprice)
                        {
                            if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) results.Add(s);
                        }
                    }
                    setprice = results;
                }

                return Ok(setprice);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/setprice/currency")]
        public ActionResult<List<Models.GenericList>> GetSetPriceCurrency()
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_EgswCurrencyGetList";
                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = string.Empty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var list = new List<Models.GenericList>();
                foreach (DataRow p in ds.Tables[0].Rows)
                {
                    list.Add(new Models.GenericList { Code = GetInt(p["Code"]), Value = GetStr(p["Description"]) });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/setprice")]
        public ActionResult<Models.ResponseCallBack> SaveSetPrice([FromBody] Models.SetPriceData data)
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
                cmd.CommandText = "sp_EgswSetPriceUpdate";

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

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 50).Value = data.Info.Name ?? string.Empty;
                cmd.Parameters.Add("@tntCurrency", SqlDbType.TinyInt).Value = data.Info.CodeCurrency;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = tran;
                cmd.Parameters.Add("@tntType", SqlDbType.TinyInt).Value = 1;
                cmd.Parameters.Add("@intCodePurchasing", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@intmainSetprice", SqlDbType.Int).Value = data.Info.Main;
                cmd.Parameters.Add("@fltSPFactor", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@fltFactorToMain", SqlDbType.Decimal).Value = data.Info.Factor;
                cmd.Parameters["@intCode"].Direction = ParameterDirection.InputOutput;

                var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                retval.Direction = ParameterDirection.ReturnValue;

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
                        resultCode = -1;
                    }
                }

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.CommandTimeout = 120;
                cmd.ExecuteNonQuery();

                var codeSetPrice = GetInt(cmd.Parameters["@intCode"].Value, -1);
                resultCode = GetInt(retval.Value, -1);
                if (resultCode != 0)
                {
                    throw new DatabaseException($"[{resultCode}] Save SetPrice failed");
                }

                if (tran == 1 && codeSetPrice > 0)
                {
                    cmd.CommandText = "API_UPDATE_PriceFromMainByFactor";
                    cmd.CommandTimeout = 1000;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codeSetPrice;
                    cmd.ExecuteNonQuery();
                }

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = codeSetPrice;
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
                response.Message = "Save Set Price failed";
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

        [HttpPost("api/setprice/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteSetPrice([FromBody] Models.GenericDeleteData data)
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
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWSETPRICE";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                var skip = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
                skip.Direction = ParameterDirection.Output;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;
                cmd.CommandTimeout = 10000;
                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0)
                {
                    throw new DatabaseException($"[{resultCode}] Delete setprice failed");
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
                response.Message = "Delete setprice failed";
                return StatusCode(500, response);
            }
            catch (ArgumentException)
            {
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                return BadRequest(response);
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
