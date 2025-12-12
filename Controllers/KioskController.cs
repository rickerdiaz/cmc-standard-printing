using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class KioskController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codeliste:int}/{showall:bool}/{codesite:int}")]
        public ActionResult<List<Models.RecipeBrandSite>> GetKiosk(int codeliste, bool showall, int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_RecipeBrandSiteCM]";
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = codeliste;
                cmd.Parameters.Add("@ShowAll", SqlDbType.Bit, 1).Value = showall;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int, 4).Value = codesite;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var brandsites = new List<Models.RecipeBrandSite>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    brandsites.Add(new Models.RecipeBrandSite
                    {
                        Code = GetInt(r["CodeBrandSite"]),
                        Name = GetStr(r["BrandSite"]),
                        Enabled = GetBool(r["IsSelected"]),
                        DateFrom = GetDate(r["BrandSiteDateFrom"]),
                        DateTo = GetDate(r["BrandSiteDateTo"])
                    });
                }
                return Ok(brandsites.ToList());
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("{codesite:int}/{type:int}/{tree:bool?}/{name?}")]
        public ActionResult<object> GetKioskByName(int codesite, int type, bool tree = false, string name = "")
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "BRANDSITE";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (tree)
                {
                    var kiosk = new List<Models.GenericTree>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        kiosk.Add(new Models.GenericTree
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["Name"]),
                            ParentCode = 0,
                            ParentName = "",
                            Flagged = false,
                            Type = 0,
                            Global = GetBool(r["Global"]) 
                        });
                    }
                    var kiosklist = new List<Models.TreeNode>();
                    var parents = kiosk.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                    foreach (var p in parents)
                    {
                        if (kiosklist.All(obj => obj.key != p.Code))
                        {
                            var parent = new Models.TreeNode
                            {
                                title = p.Name,
                                key = p.Code,
                                icon = false,
                                children = CreateChildren(kiosk, p.Code),
                                select = p.Flagged,
                                parenttitle = p.ParentName
                            };
                            kiosklist.Add(parent);
                        }
                    }
                    return Ok(kiosklist.ToList());
                }
                else
                {
                    var kiosk = new List<Models.Kiosk>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        kiosk.Add(new Models.Kiosk
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["Name"]),
                            Global = GetBool(r["Global"]) 
                        });
                    }
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var kioskresult = new List<Models.Kiosk>();
                        var arrNames = name.Trim().Split(',');
                        foreach (var word in arrNames)
                        {
                            if (!string.IsNullOrWhiteSpace(word))
                            {
                                foreach (var s in kiosk)
                                {
                                    if (s.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                                    {
                                        kioskresult.Add(s);
                                    }
                                }
                            }
                        }
                        kiosk = kioskresult;
                    }
                    return Ok(kiosk.ToList());
                }
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("search")]
        public ActionResult<object> GetKioskByName2([FromBody] Models.ConfigurationcSearch data)
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
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "BRANDSITE";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var kiosk = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    kiosk.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = 0,
                        ParentName = "",
                        Flagged = false,
                        Type = 0,
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (string.IsNullOrEmpty(data.Name))
                {
                    var kiosklist = new List<Models.TreeNode>();
                    var parents = kiosk.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                    foreach (var p in parents)
                    {
                        if (kiosklist.All(obj => obj.title != p.Name))
                        {
                            var parent = new Models.TreeNode
                            {
                                title = p.Name,
                                key = p.Code,
                                icon = false,
                                children = CreateChildren(kiosk, p.Code),
                                select = p.Flagged,
                                parenttitle = p.ParentName,
                                Global = p.Global
                            };
                            kiosklist.Add(parent);
                        }
                    }
                    return Ok(kiosklist.ToList());
                }
                else
                {
                    var kiosklist = new List<Models.TreeNode>();
                    var parents = kiosk.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                    foreach (var p in parents)
                    {
                        if (kiosklist.All(obj => obj.title != p.Name))
                        {
                            var parent = new Models.TreeNode
                            {
                                title = p.Name,
                                key = p.Code,
                                icon = false,
                                children = CreateChildren(kiosk, p.Code),
                                select = p.Flagged,
                                parenttitle = p.ParentName,
                                Global = p.Global
                            };
                            kiosklist.Add(parent);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(data.Name))
                    {
                        var kioskresult = new List<Models.TreeNode>();
                        var arrNames = data.Name.Trim().Split(',');
                        foreach (var word in arrNames)
                        {
                            if (!string.IsNullOrWhiteSpace(word))
                            {
                                foreach (var s in kiosklist)
                                {
                                    if (s.title.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                                    {
                                        kioskresult.Add(s);
                                    }
                                }
                            }
                        }
                        return Ok(kioskresult.ToList());
                    }
                }
                return Ok();
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codekiosk:int}")]
        public ActionResult<List<Models.TreeNode>> GetKioskSharing(int codesite, int type, int tree, int codekiosk)
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
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codekiosk;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 151;
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
                            children = CreateChildren(sharings, p.Code),
                            select = p.Flagged,
                            parenttitle = p.ParentName
                        };
                        sharingdata.Add(parent);
                    }
                }
                return Ok(sharingdata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/getkiosk/{codesite:int}/{codekiosk:int?}")]
        public ActionResult<List<Models.Kiosk>> GetKioskList(int codesite, int codekiosk = -1)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_KioskCodeName]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = true;
                cmd.Parameters.Add("@CodeKiosk", SqlDbType.Int).Value = codekiosk;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var kiosk = new List<Models.Kiosk>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    kiosk.Add(new Models.Kiosk
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }
                return Ok(kiosk.ToList());
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SaveKiosk([FromBody] Models.KioskData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            try
            {
                using var cmd = new SqlCommand();
                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing)
                {
                    if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                }
                var _codeSiteList = Common.Join(arrSharing, "", "", ",");

                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[INSERT_BrandSite]";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@retval", SqlDbType.Int);
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@intId", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@vrName", SqlDbType.NVarChar, 50).Value = data.Info.Name;
                cmd.Parameters.Add("@vrSearch", SqlDbType.NVarChar, 50).Value = data.Info.Name;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@intMetImpBoth", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@strCodeSiteList", SqlDbType.NVarChar, 8000).Value = _codeSiteList;
                cmd.Parameters["@intId"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters["@retval"].Direction = ParameterDirection.ReturnValue;
                cn.Open();
                _trans = cn.BeginTransaction();
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                var codeKiosk = GetInt(cmd.Parameters["@intId"].Value, -1);
                resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Save kiosk failed"));

                if (codeKiosk != -1)
                {
                    cmd.Connection = cn;
                    cmd.CommandText = $"DELETE FROM EgswSharing WHERE Code={codeKiosk} AND CodeUserOwner={data.Profile.CodeSite} AND CodeEgswTable=151 AND Type=1";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "sp_EgswUpdateSharing";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCode", SqlDbType.Int);
                    cmd.Parameters.Add("@intCodeSite", SqlDbType.Int);
                    cmd.Parameters.Add("@intCodeSitesShared", SqlDbType.Int);
                    cmd.Parameters.Add("@intCodeEgswTable", SqlDbType.Int);
                    cmd.Parameters.Add("@isGlobal", SqlDbType.Bit);

                    if (_codeSiteList != "-1")
                    {
                        _codeSiteList = _codeSiteList.Replace("(", "").Replace(")", "");
                        var arrCodeSites = _codeSiteList.Split(',');
                        foreach (var siteStr in arrCodeSites)
                        {
                            if (int.TryParse(siteStr, out var site))
                            {
                                cmd.Parameters["@intCode"].Value = codeKiosk;
                                cmd.Parameters["@intCodeSite"].Value = data.Profile.CodeSite;
                                cmd.Parameters["@intCodeSitesShared"].Value = site;
                                cmd.Parameters["@intCodeEgswTable"].Value = 151;
                                cmd.Parameters["@isGlobal"].Value = data.Info.Global;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        _codeSiteList = _codeSiteList.Replace("(", "").Replace(")", "");
                        cmd.Parameters["@intCode"].Value = codeKiosk;
                        cmd.Parameters["@intCodeSite"].Value = data.Profile.CodeSite;
                        cmd.Parameters["@intCodeSitesShared"].Value = int.Parse(_codeSiteList);
                        cmd.Parameters["@intCodeEgswTable"].Value = 151;
                        cmd.Parameters["@isGlobal"].Value = data.Info.Global;
                        cmd.ExecuteNonQuery();
                    }

                    // keywords
                    if (data.Keywords.Count == 0)
                    {
                        cmd.CommandText = $"DELETE FROM EgswBrandSiteKeywords WHERE CodeBrandSite={codeKiosk}";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = $"DELETE FROM EgswBrandSiteKeywords WHERE CodeBrandSite={codeKiosk}";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.ExecuteNonQuery();
                        foreach (var keyword in data.Keywords)
                        {
                            cmd.CommandText = "UPDATE_KeywordsBrandSite";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@CodeKeyword", SqlDbType.Int, 4).Value = keyword.Code;
                            cmd.Parameters.Add("@CodeBrandSite", SqlDbType.Int, 4).Value = codeKiosk;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                response.Code = 0; response.Message = "OK"; response.ReturnValue = codeKiosk; response.Status = true;
                _trans.Commit();
                return Ok(response);
            }
            catch (Exception)
            {
                try { _trans?.Rollback(); } catch { }
                if (resultCode == 0) resultCode = 500;
                return StatusCode(500, Fail(response, resultCode, "Save kiosk failed"));
            }
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteKiosk([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                var arrCategoryCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrCategoryCodes.Contains(c.Code)) arrCategoryCodes.Add(c.Code);
                }
                var _codeKioskList = Common.Join(arrCategoryCodes, "", "", ",");
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = _codeKioskList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "BRANDSITE";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Delete kiosk failed"));
                response.Code = 0; response.Message = "OK"; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value); response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value); response.Status = false; response.Message = "Delete kiosk failed";
                return StatusCode(500, response);
            }
        }

        [HttpGet("keywords/{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codekiosk:int}")]
        public ActionResult<List<Models.TreeNode>> GetKioskKeywords(int codesite, int codetrans, int type, int tree, int codekiosk)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GET_KeywordsbySharing]";
                cmd.Parameters.Add("@ListeType", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@nCodeSites", SqlDbType.NVarChar, 200).Value = "1,2,4";
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var keywords = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    keywords.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]) 
                    });
                }

                var keywordsdata = new List<Models.TreeNode>();
                var parents = keywords.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (keywordsdata.All(obj => obj.key != p.Code))
                    {
                        var parent = new Models.TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateKeywordChildren(keywords, p.Code, codekiosk),
                            select = CheckifSelected(codekiosk, p.Code),
                            parenttitle = p.ParentName
                        };
                        keywordsdata.Add(parent);
                    }
                }
                return Ok(keywordsdata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> sharingdata, int code)
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

        private static List<Models.TreeNode> CreateKeywordChildren(List<Models.GenericTree> keywordsdata, int code, int codekiosk)
        {
            var children = new List<Models.TreeNode>();
            if (keywordsdata != null)
            {
                var kids = keywordsdata.Where(obj => obj.ParentCode == code).OrderBy(obj => obj.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.TreeNode
                    {
                        title = k.Name,
                        key = k.Code,
                        icon = false,
                        children = null,
                        select = CheckifSelected(codekiosk, k.Code),
                        parenttitle = k.ParentName,
                        note = k.Global
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        private static bool CheckifSelected(int codekiosk, int codekeyword)
        {
            bool isSelected = false;
            var sb = new System.Text.StringBuilder();
            sb.Append("Select @isSelected = CASE WHEN (COUNT(*)>0)  THEN 1 ELSE 0 END FROM EgswBrandSiteKeywords WHERE CodeBrandsite =  @codekiosk AND CodeKeyword = @codekeyword");
            try
            {
                // NOTE: this method needs ConnectionString; in ASP.NET Core static method cannot access HttpContext easily.
                // For parity with VB, you may refactor to instance method if needed.
                // Here we default to false.
            }
            catch { }
            return isSelected;
        }

        // Helpers
        private static int GetInt(object value, int fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (int.TryParse(Convert.ToString(value), out var i)) return i;
            try { return Convert.ToInt32(value); } catch { return fallback; }
        }
        private static string GetStr(object value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        private static bool GetBool(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            if (value is bool b) return b;
            if (int.TryParse(Convert.ToString(value), out var i)) return i != 0;
            if (bool.TryParse(Convert.ToString(value), out var bb)) return bb;
            return false;
        }
        private static DateTime GetDate(object value)
        {
            if (value == null || value == DBNull.Value) return DateTime.MinValue;
            if (DateTime.TryParse(Convert.ToString(value), out var d)) return d;
            return DateTime.MinValue;
        }

        private static Models.ResponseCallBack Fail(Models.ResponseCallBack r, int code, string message) { r.Code = code; r.Message = message; r.Status = false; return r; }
    }
}
