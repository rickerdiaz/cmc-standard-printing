using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.ProductionLocations;
using CmcStandardPrinting.Domain.Sources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionLocationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProductionLocationController> _logger;

    public ProductionLocationController(IConfiguration configuration, ILogger<ProductionLocationController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("search")]
    public ActionResult<List<ProductionLocation>> GetProductionLocation([FromBody] ConfigurationcSearch data)
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

            var prodloc = MapProductionLocations(ds.Tables.Count > 0 ? ds.Tables[0] : null);
            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                prodloc = FilterByName(prodloc, data.Name);
            }

            return Ok(prodloc);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProductionLocation: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/getproductionlocation/{codesite:int}/{codeproductionlocation:int?}")]
    public ActionResult<List<ProductionLocation>> GetProductionLocationPerItem(int codesite, int? codeproductionlocation = -1)
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
            cmd.Parameters.Add("@CodeProductionLocation", SqlDbType.Int).Value = codeproductionlocation ?? -1;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var prodloc = MapProductionLocations(ds.Tables.Count > 0 ? ds.Tables[0] : null);
            return Ok(prodloc);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProductionLocationPerItem: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codesource:int}")]
    public ActionResult<List<TreeNode>> GetProductionLocationSharing(int codesite, int type, int tree, int codesource)
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

            var sharings = new List<GenericTree>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new GenericTree
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
            }

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (sharingdata.All(obj => obj.key != p.Code))
                {
                    var parent = new TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildrenSharing(sharings, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        GroupLevel = GroupLevel.Property
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProductionLocationSharing: database error");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveProductionLocation([FromBody] ProductionLocationData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlTransaction? transaction = null;
        var strCodesToMerge = string.Empty;
        try
        {
            if (data.MergeList != null && data.MergeList.Count > 0)
            {
                foreach (var s in data.MergeList)
                {
                    strCodesToMerge = string.IsNullOrEmpty(strCodesToMerge) ? s.ToString(CultureInfo.InvariantCulture) : strCodesToMerge + "," + s;
                }
            }

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing ?? new List<GenericList>())
            {
                if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
            }

            var codeSiteList = Join(arrSharing, "(", ")", ",");
            var tran = (data.Info.Code == -1 || data.Info.Code == -2) ? 1 : 2;

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
            transaction = cn.BeginTransaction();
            cmd.Transaction = transaction;
            cmd.ExecuteNonQuery();
            var codeSource = GetInt(cmd.Parameters["@intCode"].Value, -1);
            resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
            if (resultCode != 0)
            {
                transaction.Rollback();
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save Production Location failed";
                response.ReturnValue = string.Empty;
                return StatusCode(500, response);
            }

            if (data.ActionType == 5 && data.MergeList != null && data.MergeList.Count > 0)
            {
                var arrSites = new ArrayList();
                foreach (var s in data.MergeList)
                {
                    if (!arrSites.Contains(s)) arrSites.Add(s);
                }

                var siteList = Join(arrSites, "(", ")", ",");
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
                var mergeList = Join(arrMergeList, "(", ")", ",");
                sql.Clear();
                sql.Append("UPDATE EgswListe ");
                sql.Append("SET Source=@newSoureeCode ");
                sql.Append(" WHERE Source IN " + mergeList);
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@newSoureeCode", SqlDbType.Int).Value = codeSource;
                _ = cmd.ExecuteNonQuery();

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
                        transaction.Rollback();
                        response.Code = resultCode;
                        response.Status = false;
                        response.Message = "Delete merged category failed";
                        response.ReturnValue = string.Empty;
                        return StatusCode(500, response);
                    }
                }
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = codeSource;
            response.Status = true;
            transaction.Commit();
            return Ok(response);
        }
        catch (Exception ex)
        {
            try { transaction?.Rollback(); } catch { /* ignore rollback issues */ }
            if (resultCode == 0) resultCode = 500;
            var errorResponse = new ResponseCallBack
            {
                Code = resultCode,
                Message = "Save Production Location failed",
                Status = false,
                ReturnValue = string.Empty
            };
            _logger.LogError(ex, "SaveProductionLocation: database error");
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeleteProductionLocation([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            var arrCategoryCodes = new ArrayList();
            foreach (var c in data.CodeList ?? new List<DeleteCode>())
            {
                if (!arrCategoryCodes.Contains(c.Code)) arrCategoryCodes.Add(c.Code);
            }

            var codeCategoryList = Join(arrCategoryCodes, string.Empty, string.Empty, ",");
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
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Delete source failed";
                response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                return StatusCode(500, response);
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Unexpected error occured";
            _logger.LogError(ex, "DeleteProductionLocation: database error");
            return StatusCode(500, response);
        }
    }

    [HttpGet("/api/getrecipeproductionlocation/{codesite:int}")]
    public ActionResult<List<GenericCodeValueList>> GetProductionLocation(int codesite)
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

            var list = new List<GenericCodeValueList>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new GenericCodeValueList
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["Name"])
                    });
                }
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProductionLocation generic list: database error");
            return StatusCode(500);
        }
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> sharingdata, int code)
    {
        var children = new List<TreeNode>();
        var kids = sharingdata.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
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

        return children;
    }

    private static List<ProductionLocation> MapProductionLocations(DataTable? table)
    {
        var result = new List<ProductionLocation>();
        if (table == null) return result;

        foreach (DataRow r in table.Rows)
        {
            result.Add(new ProductionLocation
            {
                Code = GetInt(r["Code"]),
                Name = GetStr(r["Name"]),
                IsGlobal = GetBool(r["IsGlobal"])
            });
        }

        return result;
    }

    private static List<ProductionLocation> FilterByName(List<ProductionLocation> productionLocations, string name)
    {
        var result = new List<ProductionLocation>();
        foreach (var word in name.Split(','))
        {
            var trimmed = (word ?? string.Empty).Trim();
            if (trimmed.Length == 0) continue;
            var w = ReplaceSpecialCharacters(trimmed.ToLowerInvariant());
            foreach (var s in productionLocations)
            {
                if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(w)) result.Add(s);
            }
        }

        return result;
    }

    private static string Join(ArrayList list, string prefix, string suffix, string separator)
    {
        var val = string.Empty;
        foreach (var c in list)
        {
            if (!string.IsNullOrEmpty(val)) val += separator;
            val += c?.ToString();
        }

        return string.IsNullOrEmpty(val) ? string.Empty : prefix + val + suffix;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
        try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
    }

    private static string GetStr(object? value, string fallback = "")
    {
        if (value == null || value == DBNull.Value) return fallback;
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var result = value;
        var specialChars = new Dictionary<string, string>
        {
            { "ä", "ae" },
            { "ö", "oe" },
            { "ü", "ue" },
            { "ß", "ss" },
            { "é", "e" },
            { "è", "e" },
            { "ê", "e" },
            { "à", "a" },
            { "á", "a" },
            { "â", "a" },
            { "ù", "u" },
            { "û", "u" },
            { "ú", "u" },
            { "î", "i" },
            { "ï", "i" }
        };

        foreach (var k in specialChars.Keys)
        {
            result = result.Replace(k, specialChars[k], StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}
