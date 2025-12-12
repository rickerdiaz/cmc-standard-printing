using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CalcmenuAPI
{
    [ApiController]
    public class SiteController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/sites")]
        public ActionResult<List<Models.TreeNode>> GetSites()
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

                var sharings = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Type = GetInt(r["Type"]) 
                    });
                }
                var groupLevel = GetInt(ds.Tables[0].Rows[0]["GroupLevel"]);

                var sharingdata = new List<Models.TreeNode>();
                var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildrenSharing(sharings, p.Code),
                        parenttitle = p.ParentName,
                        groupLevel = groupLevel
                    };
                    sharingdata.Add(parent);
                }

                return Ok(sharingdata);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/site/{codesite:int}/{codetrans:int}")]
        public ActionResult<Models.SiteData> GetSite(int codesite, int codetrans)
        {
            try
            {
                var data = new Models.SiteData();
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Common.SP_API_GET_SiteInfo;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds != null && ds.Tables.Count == 8)
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
                        data.Translation = new List<Models.Translation>();
                        foreach (DataRow dr in tblTranslation.Rows)
                        {
                            data.Translation.Add(new Models.Translation
                            {
                                Code = GetInt(dr["Code"]),
                                Value = GetStr(dr["Value"]),
                                CodeDict = GetInt(dr["CodeDict"]) 
                            });
                        }
                    }

                    if (tblAutonumber.Rows.Count > 0)
                    {
                        data.Autonumber = new List<Models.GenericList>();
                        foreach (DataRow dr in tblAutonumber.Rows)
                        {
                            data.Autonumber.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]).ToLowerInvariant() });
                        }
                    }

                    var config = new List<Models.GenericList>();
                    if (tblConfig.Rows.Count > 0)
                    {
                        foreach (DataRow dr in tblConfig.Rows)
                        {
                            config.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]) });
                        }
                    }
                    data.Config = ProxyEncode(JsonConvert.SerializeObject(config, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    if (tblNutrientSet.Rows.Count > 0)
                    {
                        data.NutrientSet = new List<Models.GenericList>();
                        foreach (DataRow dr in tblNutrientSet.Rows)
                        {
                            data.NutrientSet.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]) });
                        }
                    }

                    if (tblSetOfPrice.Rows.Count > 0)
                    {
                        data.SetOfPrice = new List<Models.GenericList>();
                        foreach (DataRow dr in tblSetOfPrice.Rows)
                        {
                            data.SetOfPrice.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]), Value2 = GetStr(dr["Format"]), Name = GetStr(dr["Sign"]) });
                        }
                    }

                    if (tblTax.Rows.Count > 0)
                    {
                        data.Tax = new List<Models.Tax>();
                        foreach (DataRow dr in tblTax.Rows)
                        {
                            data.Tax.Add(new Models.Tax
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
                        data.Units = new List<Models.Unit>();
                        foreach (DataRow dr in tblUnits.Rows)
                        {
                            data.Units.Add(new Models.Unit
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
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/site/property/{codeproperty:int}")]
        public ActionResult<List<Models.GenericList>> GetSiteProperty(int codeproperty)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Common.SP_API_GET_Sites;
                cmd.Parameters.Add("@Group", SqlDbType.Int).Value = codeproperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var sites = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sites.Add(new Models.GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
                return Ok(sites);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/site/config/{codesite:int}/{id?}")]
        public ActionResult<List<Models.GenericList>> GetSiteConfig(int codesite, int id = -1)
        {
            var config = new List<Models.GenericList>();
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Common.SP_API_GET_Config;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -3;
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = GetInt(codesite);
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = GetInt(id, -1);
                cn.Open();
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    config.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Value"]) });
                }
                dr.Close();
                return Ok(config);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/markeditems/{codeuser:int}/{type:int?}/{picktype:int?")]
        public ActionResult<List<Models.GenericItem>> GetGroupMarked(int codeuser, int type = -1, int picktype = -1)
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

                var items = new List<Models.GenericItem>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    items.Add(new Models.GenericItem { Code = GetInt(r["Id"]), Name = GetStr(r["Name"]) });
                }
                return Ok(items);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/language/{selected:int?}/{used:int?}")]
        public ActionResult<List<Models.UsedLanguages>> GetLanguage(int selected = 0, int used = -1)
        {
            var language = new List<Models.UsedLanguages>();
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
                    language.Add(new Models.UsedLanguages { CodeRef = GetInt(dr["CodeRef"]), Language = GetStr(dr["Language"]) });
                }
                dr.Close();
                return Ok(language);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
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
        private static string ProxyEncode(string s) => s ?? string.Empty;
    }
}
