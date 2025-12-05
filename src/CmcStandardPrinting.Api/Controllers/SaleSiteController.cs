using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class SaleSiteController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SaleSiteController> _logger;

    public SaleSiteController(IConfiguration configuration, ILogger<SaleSiteController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("/api/salesite/search")]
    public ActionResult<List<SaleSite>> GetSaleSiteByName([FromBody] ConfigurationSearch data)
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

            var salesites = new List<SaleSite>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    salesites.Add(new SaleSite
                    {
                        Code = GetInt(r["Code"]),
                        LocationNumber = GetStr(r["LocationNumber"]),
                        Name = GetStr(r["Name"]),
                        Street = GetStr(r["Street"]),
                        ZipCode = GetStr(r["ZipCode"]),
                        City = GetStr(r["City"]),
                        CertificationID = GetStr(r["CertificationID"]),
                        IsProductionLocation = GetStr(r["isProductionLocation"]),
                        IsSalesSite = GetStr(r["isSalesSite"]),
                        CodeLanguage = GetStr(r["codeLanguage"])
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var results = new List<SaleSite>();
                foreach (var w in data.Name.Split(','))
                {
                    var word = (w ?? string.Empty).Trim();
                    if (word.Length == 0) continue;
                    var key = ReplaceSpecialCharacters(word.ToLowerInvariant());
                    foreach (var s in salesites)
                    {
                        if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key))
                        {
                            results.Add(s);
                        }
                    }
                }
                salesites = results;
            }

            return Ok(salesites);
        }
        catch (ArgumentException aex)
        {
            _logger.LogWarning(aex, "GetSaleSiteByName: Missing or invalid parameters");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSaleSiteByName: Unexpected error occurred");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/salesite/sharing/{codesite:int}/{type:int}/{tree:int}/{codesalesite:int}")]
    public ActionResult<List<TreeNode>> GetSaleSiteSharing(int codesite, int type, int tree, int codesalesite)
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
            var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
            foreach (var p in parents)
            {
                if (sharingdata.All(o => o.Key != p.Code))
                {
                    var parent = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildrenSharing(sharings, p.Code),
                        Select = p.Flagged,
                        Selected = p.Flagged,
                        ParentTitle = p.ParentName
                    };
                    if (parent.Children != null && parent.Children.Count > 0)
                    {
                        sharingdata.Add(parent);
                    }
                }
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException aex)
        {
            _logger.LogWarning(aex, "GetSaleSiteSharing: Missing or invalid parameters");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSaleSiteSharing: Unexpected error occurred");
            return StatusCode(500);
        }
    }

    [HttpPost("api/salesite")]
    public ActionResult<ResponseCallBack> SaveSaleSite([FromBody] SaleSiteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlTransaction? trans = null;
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
            cmd.Parameters.Add("@ZipCode", SqlDbType.Int).Value = GetInt(data.Info.ZipCode);
            cmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = data.Info.City;
            cmd.Parameters.Add("@CertificationID", SqlDbType.NVarChar).Value = data.Info.CertificationID;
            cmd.Parameters.Add("@IsProductionLocation", SqlDbType.Bit).Value = GetBool(data.Info.IsProductionLocation);
            cmd.Parameters.Add("@IsSalesSite", SqlDbType.Bit).Value = GetBool(data.Info.IsSalesSite);
            cmd.Parameters.Add("@Codelanguage", SqlDbType.Int).Value = GetInt(data.Info.CodeLanguage);

            var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
            retval.Direction = ParameterDirection.ReturnValue;

            var arrSharing = new ArrayList();
            var codeSiteList = Join(arrSharing, "(", ")", ",");
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
        catch (DatabaseException ex)
        {
            try
            {
                trans?.Rollback();
                trans?.Dispose();
            }
            catch
            {
            }

            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Save Sale Site failed";
            _logger.LogError(ex, "SaveSaleSite: Database error occured");
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            _logger.LogError(ex, "SaveSaleSite: Unexpected error occured");
            return StatusCode(500, response);
        }
    }

    [HttpGet("/api/salesite/getsalesite/{code:int}/{codesite:int}")]
    public ActionResult<List<SaleSite>> GetSaleSite(int code, int codesite)
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

            var salessite = new List<SaleSite>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    salessite.Add(new SaleSite
                    {
                        Code = GetInt(r["Code"]),
                        LocationNumber = GetStr(r["LocationNumber"]),
                        Name = GetStr(r["Name"]),
                        Street = GetStr(r["Street"]),
                        ZipCode = GetStr(r["ZipCode"]),
                        City = GetStr(r["City"]),
                        CertificationID = GetStr(r["CertificationID"]),
                        IsProductionLocation = GetBool(r["isProductionLocation"]).ToString(CultureInfo.InvariantCulture),
                        IsSalesSite = GetBool(r["isSalesSite"]).ToString(CultureInfo.InvariantCulture),
                        CodeLanguage = GetStr(r["codeLanguage"])
                    });
                }
            }

            return Ok(salessite);
        }
        catch (ArgumentException aex)
        {
            _logger.LogWarning(aex, "GetSaleSite: Missing or invalid parameters");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSaleSite: Unexpected error occurred");
            return StatusCode(500);
        }
    }

    [HttpPost("api/salesite/delete")]
    public ActionResult<ResponseCallBack> DeleteSaleSite([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            var codeList = (data.CodeList ?? new List<DeleteCode>()).Select(c => c.Code).Distinct().ToList();
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
        catch (DatabaseException ex)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Delete sales site failed";
            _logger.LogError(ex, "DeleteSaleSite: Database error occured");
            return StatusCode(500, response);
        }
        catch (ArgumentException)
        {
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            response.Parameters = new List<Param> { new() { Name = "data", Value = "SaleSiteData" } };
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            _logger.LogError(ex, "DeleteSaleSite: Unexpected error occured");
            return StatusCode(500, response);
        }
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> nodes, int code)
    {
        var children = new List<TreeNode>();
        if (nodes == null) return children;
        var kids = nodes.Where(o => o.Code != code && o.ParentCode == code && code > 0).OrderBy(o => o.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = CreateChildrenSharing(nodes, k.Code),
                Select = k.Flagged,
                Selected = k.Flagged,
                ParentTitle = k.ParentName
            };
            children.Add(child);
        }
        return children;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

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
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        return false;
    }

    private static string Join(ArrayList arr, string strPrefix, string strSuffix, string delimiter, string valuePrefixAndSuffix = "")
    {
        var i = 0;
        var str = string.Empty;
        while (i < arr.Count)
        {
            str += valuePrefixAndSuffix + arr[i] + valuePrefixAndSuffix + delimiter;
            i += 1;
        }

        if (str.Length == 0) return string.Empty;
        str = strPrefix + str.Substring(0, str.Length - 1) + strSuffix;
        return str;
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        var specials = new[] {"[[]TM]", "[[]tm]", "[[]R]", "[[]r]", "[[]C]", "[[]c]", "[TM]", "[tm]", "[R]", "[r]", "[C]", "[c]"};
        var replacements = new[] {"™", "™", "®", "®", "©", "©", "™", "™", "®", "®", "©", "©"};

        for (var i = 0; i < specials.Length; i++)
        {
            value = Regex.Replace(value, Regex.Escape(specials[i]), replacements[i], RegexOptions.IgnoreCase);
        }

        return value;
    }
}

internal sealed class DatabaseException : Exception
{
    public DatabaseException(string message) : base(message)
    {
    }
}
