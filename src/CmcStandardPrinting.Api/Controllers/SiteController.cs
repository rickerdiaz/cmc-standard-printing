using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Sites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class SiteController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SiteController> _logger;

    public SiteController(IConfiguration configuration, ILogger<SiteController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("/api/sites")]
    public ActionResult<List<TreeNode>> GetSites()
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Sites_Generic]";
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
                        Type = GetInt(r["Type"]),
                        Global = GetBool(r["Global"])
                    });
                }
            }

            var groupLevelRaw = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? GetInt(ds.Tables[0].Rows[0]["GroupLevel"]) : 0;
            var groupLevel = Enum.IsDefined(typeof(GroupLevel), groupLevelRaw) ? (GroupLevel)groupLevelRaw : GroupLevel.Property;

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new TreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = CreateChildrenSharing(sharings, p.Code),
                    ParentTitle = p.ParentName,
                    GroupLevel = groupLevel
                };
                sharingdata.Add(parent);
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetSites: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetSites: Unexpected error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/site/{codesite:int}/{codetrans:int}")]
    public ActionResult<SiteData> GetSite(int codesite, int codetrans)
    {
        try
        {
            var data = new SiteData();
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_SiteInfo";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (ds.Tables.Count == 8)
            {
                var tblInfo = ds.Tables[0];
                var tblTranslation = ds.Tables[1];
                var tblAutonumber = ds.Tables[2];
                var tblConfig = ds.Tables[3];
                var tblNutrientSet = ds.Tables[4];
                var tblSetOfPrice = ds.Tables[5];
                var tblTax = ds.Tables[6];
                var tblUnits = ds.Tables[7];

                foreach (DataRow dr in tblInfo.Rows)
                {
                    data.Info.Name = GetStr(dr["Name"]);
                    data.Info.RefName = GetStr(dr["RefName"]);
                    data.Info.Group = GetStr(dr["Group"]);
                    data.Info.SiteLevel = GetStr(dr["SiteLevel"]);
                }

                if (tblTranslation.Rows.Count > 0)
                {
                    data.Translation = new List<SiteTranslation>();
                    foreach (DataRow dr in tblTranslation.Rows)
                    {
                        data.Translation.Add(new SiteTranslation
                        {
                            Code = GetInt(dr["Code"]),
                            Value = GetStr(dr["Value"]),
                            CodeDict = GetInt(dr["CodeDict"])
                        });
                    }
                }

                if (tblAutonumber.Rows.Count > 0)
                {
                    data.Autonumber = new List<GenericListItem>();
                    foreach (DataRow dr in tblAutonumber.Rows)
                    {
                        data.Autonumber.Add(new GenericListItem { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]).ToLowerInvariant() });
                    }
                }

                var config = new List<GenericListItem>();
                if (tblConfig.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblConfig.Rows)
                    {
                        config.Add(new GenericListItem { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]) });
                    }
                }

                data.Config = ProxyEncode(JsonConvert.SerializeObject(config, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                if (tblNutrientSet.Rows.Count > 0)
                {
                    data.NutrientSet = new List<GenericListItem>();
                    foreach (DataRow dr in tblNutrientSet.Rows)
                    {
                        data.NutrientSet.Add(new GenericListItem { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]) });
                    }
                }

                if (tblSetOfPrice.Rows.Count > 0)
                {
                    data.SetOfPrice = new List<GenericListItem>();
                    foreach (DataRow dr in tblSetOfPrice.Rows)
                    {
                        data.SetOfPrice.Add(new GenericListItem { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]), Value2 = GetStr(dr["Format"]), Name = GetStr(dr["Sign"]) });
                    }
                }

                if (tblTax.Rows.Count > 0)
                {
                    data.Tax = new List<Tax>();
                    foreach (DataRow dr in tblTax.Rows)
                    {
                        data.Tax.Add(new Tax
                        {
                            TaxCode = GetInt(dr["Code"]),
                            TaxValue = GetDbl(dr["Value"]),
                            TaxName = GetStr(dr["Description"]),
                            Global = GetBool(dr["Global"])
                        });
                    }
                }

                if (tblUnits.Rows.Count > 0)
                {
                    data.Units = new List<Unit>();
                    foreach (DataRow dr in tblUnits.Rows)
                    {
                        data.Units.Add(new Unit
                        {
                            Code = GetInt(dr["CodeUnit"]),
                            Value = GetStr(dr["UnitName"]),
                            Type = GetInt(dr["Type"]),
                            TypeMain = GetInt(dr["TypeMain"]),
                            FactorToMain = GetDbl(dr["FactorToMain"]),
                            IsMetric = GetInt(dr["IsMetric"]),
                            IsIngredient = GetBool(dr["IsIngredient"]),
                            IsYield = GetBool(dr["IsYield"]),
                            Format = GetStr(dr["Format"])
                        });
                    }
                }
            }

            return Ok(data);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetSite: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetSite: Unexpected error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/site/property/{codeproperty:int}")]
    public ActionResult<List<GenericListItem>> GetSiteProperty(int codeproperty)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_Sites";
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = codeproperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var sites = new List<GenericListItem>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sites.Add(new GenericListItem { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
            }

            return Ok(sites);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetSiteProperty: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetSiteProperty: Unexpected error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/site/config/{codesite:int}/{id?}")]
    public ActionResult<List<GenericListItem>> GetSiteConfig(int codesite, int id = -1)
    {
        var config = new List<GenericListItem>();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_Config";
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -3;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = GetInt(codesite);
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = GetInt(id, -1);
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                config.Add(new GenericListItem { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]) });
            }
            dr.Close();
            return Ok(config);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetSiteConfig: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetSiteConfig: Unexpected error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/markeditems/{codeuser:int}/{type:int?}/{picktype:int?}")]
    public ActionResult<List<GenericItem>> GetGroupMarked(int codeuser, int type = -1, int picktype = -1)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_MarkedItems]";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@PickType", SqlDbType.Int).Value = picktype;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var items = new List<GenericItem>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    items.Add(new GenericItem { Code = GetInt(r["Id"]), Name = GetStr(r["Name"]) });
                }
            }

            return Ok(items);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetGroupMarked: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetGroupMarked: Unexpected error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/language/{selected:int?}/{used:int?}")]
    public ActionResult<List<UsedLanguage>> GetLanguage(int selected = 0, int used = -1)
    {
        var language = new List<UsedLanguage>();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_EgswLanguageGetList]";
            cmd.Parameters.Add("@Selected", SqlDbType.Int).Value = GetInt(selected);
            cmd.Parameters.Add("@Used", SqlDbType.Int).Value = GetInt(used, -1);
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                language.Add(new UsedLanguage { CodeRef = GetInt(dr["CodeRef"]), Language = GetStr(dr["Language"]) });
            }
            dr.Close();
            return Ok(language);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetLanguage: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetLanguage: Unexpected error occurred", ex);
            return StatusCode(500);
        }
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> sharingdata, int code)
    {
        var children = new List<TreeNode>();
        if (sharingdata != null)
        {
            var kids = sharingdata.Where(o => o.ParentCode == code && o.Type == 2).OrderBy(o => o.Name).ToList();
            foreach (var k in kids)
            {
                var child = new TreeNode
                {
                    Title = k.Name,
                    Key = k.Code,
                    Icon = false,
                    Children = null,
                    Select = k.Flagged,
                    ParentTitle = k.ParentName,
                    Note = k.Global
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
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }

    private static bool GetBool(object value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private static string ProxyEncode(string s) => s ?? string.Empty;

    private void LogWarn(string msg, Exception ex) => _logger.LogWarning(ex, msg);
    private void LogError(string msg, Exception ex) => _logger.LogError(ex, msg);
}
