using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionLocationController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpPost("search")]
        public ActionResult<List<Models.ProductionLocation>> GetProductionLocation([FromBody] Models.ConfigurationcSearch data)
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWPRODUCTIONLOCATION";
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var prodloc = new List<Models.ProductionLocation>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    prodloc.Add(new Models.ProductionLocation
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        IsGlobal = GetBool(r["IsGlobal"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var result = new List<Models.ProductionLocation>();
                    foreach (var word in data.Name.Split(','))
                    {
                        if (string.IsNullOrWhiteSpace(word)) continue;
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var s in prodloc)
                        {
                            if (s.Name.ToLowerInvariant().Contains(w)) result.Add(s);
                        }
                    }
                    prodloc = result;
                }

                return Ok(prodloc);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/getproductionlocation/{codesite:int}/{codeproductionlocation:int?}")]
        public ActionResult<List<Models.ProductionLocation>> GetProductionLocationPerItem(int codesite, int codeproductionlocation = -1)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_ProductionLocationData]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeProductionLocation", SqlDbType.Int).Value = codeproductionlocation;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var prodloc = new List<Models.ProductionLocation>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    prodloc.Add(new Models.ProductionLocation
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        IsGlobal = GetBool(r["IsGlobal"]) 
                    });
                }

                return Ok(prodloc);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codesource:int}")]
        public ActionResult<List<Models.TreeNode>> GetProductionLocationSharing(int codesite, int type, int tree, int codesource)
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
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 158;
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
                            groupLevel = GroupLevel.Property
                        };
                        if (parent.children != null && parent.children.Count > 0)
                            sharingdata.Add(parent);
                    }
                }

                return Ok(sharingdata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SaveProductionLocation([FromBody] Models.ProductionLocationData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            string strCodesToMerge = string.Empty;
            try
            {
                if (data.MergeList != null && data.MergeList.Count > 0)
                {
                    foreach (var s in data.MergeList)
                    {
                        strCodesToMerge = string.IsNullOrEmpty(strCodesToMerge) ? s.ToString() : strCodesToMerge + "," + s;
                    }
                }

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing)
                {
                    if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                }
                var codeSiteList = Common.Join(arrSharing, "(", ")", ",");
                int tran = (data.Info.Code == -1 || data.Info.Code == -2) ? 1 : 2;

                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[sp_EgswProductionLocationInsertUpdate]";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@retval", SqlDbType.Int);
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 50).Value = data.Info.Name;
                cmd.Parameters.Add("@EGSID", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.IsGlobal;
                cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = tran;
                cmd.Parameters.Add("@vchCodeSiteList", SqlDbType.NVarChar, 8000).Value = codeSiteList;
                cmd.Parameters.Add("@CodesToMerge", SqlDbType.NVarChar, 2000).Value = strCodesToMerge;
                cmd.Parameters["@intCode"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters["@retval"].Direction = ParameterDirection.ReturnValue;

                cn.Open();
                _trans = cn.BeginTransaction();
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                var codeSource = GetInt(cmd.Parameters["@intCode"].Value, -1);
                resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
                if (resultCode != 0)
                {
                    _trans.Rollback();
                    response.Code = resultCode; response.Status = false; response.Message = "Save Production Location failed"; response.ReturnValue = string.Empty;
                    return StatusCode(500, response);
                }

                if (data.ActionType == 5 && data.MergeList != null && data.MergeList.Count > 0)
                {
                    var arrSites = new ArrayList();
                    foreach (var s in data.MergeList) if (!arrSites.Contains(s)) arrSites.Add(s);
                    var siteList = Common.Join(arrSites, "(", ")", ",");
                    var sql = new StringBuilder();
                    sql.Append("INSERT INTO EgswSharing ");
                    sql.Append("SELECT @Code,0,CodeUserSharedTo,1,117,0,1,0 ");
                    sql.Append("FROM EgswSharing ");
                    sql.Append("WHERE Code=@Code AND CodeEgswTable=117 AND [Status]=1 AND [Type] IN (1,5) ");
                    sql.Append("    AND CodeUserSharedTo NOT IN (");
                    sql.Append("    SELECT DISTINCT CodeUserSharedTo ");
                    sql.Append("    FROM EgswSharing  ");
                    sql.Append("    WHERE Code IN " + siteList + " AND CodeEgswTable=117 AND [Status]=1 AND [Type] IN (1,5)");
                    sql.Append(") ");
                    cmd.CommandText = sql.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codeSource;
                    cmd.ExecuteNonQuery();

                    var arrMergeList = new ArrayList();
                    foreach (var sh in data.MergeList) if (!arrMergeList.Contains(sh)) arrMergeList.Add(sh);
                    var mergeList = Common.Join(arrMergeList, "(", ")", ",");
                    sql.Clear();
                    sql.Append("UPDATE EgswListe ");
                    sql.Append("SET Source=@newSoureeCode ");
                    sql.Append(" WHERE Source IN " + mergeList);
                    cmd.CommandText = sql.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@newSoureeCode", SqlDbType.Int).Value = codeSource;
                    var rowsAffected = cmd.ExecuteNonQuery();

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
                        cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                        if (resultCode != 0)
                        {
                            _trans.Rollback();
                            response.Code = resultCode; response.Status = false; response.Message = "Delete merged category failed"; response.ReturnValue = string.Empty;
                            return StatusCode(500, response);
                        }
                    }
                }

                response.Code = 0; response.Message = "OK"; response.ReturnValue = codeSource; response.Status = true;
                _trans.Commit();
                return Ok(response);
            }
            catch (Exception)
            {
                try { _trans?.Rollback(); } catch { }
                if (resultCode == 0) resultCode = 500;
                var response = new Models.ResponseCallBack { Code = resultCode, Message = "Save Production Location failed", Status = false, ReturnValue = string.Empty };
                return StatusCode(500, response);
            }
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteProductionLocation([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrCategoryCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrCategoryCodes.Contains(c.Code)) arrCategoryCodes.Add(c.Code);
                }
                var codeCategoryList = Common.Join(arrCategoryCodes, string.Empty, string.Empty, ",");
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeCategoryList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWPRODUCTIONLOCATION";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0)
                {
                    response.Code = resultCode; response.Status = false; response.Message = "Delete source failed"; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                    return StatusCode(500, response);
                }
                response.Code = 0; response.Message = "OK"; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value); response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode; response.Status = false; response.Message = "Unexpected error occured";
                return StatusCode(500, response);
            }
        }

        [HttpGet("/api/getrecipeproductionlocation/{codesite:int}")]
        public ActionResult<List<Models.GenericCodeValueList>> GetProductionLocation(int codesite)
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWPRODUCTIONLOCATION";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var list = new List<Models.GenericCodeValueList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.GenericCodeValueList
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["Name"]) 
                    });
                }
                return Ok(list);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
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
