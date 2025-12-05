using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicationController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codesite:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.TreeNode>> GetPublication(int codesite, int codeproperty = -1, string name = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Placement]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var placements = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    placements.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["isParent"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var placementschildren = new List<Models.GenericTree>();
                    var placementsresult = new List<Models.GenericTree>();
                    foreach (var word in name.Split(','))
                    {
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var k in placements)
                        {
                            if (k.Name.ToLowerInvariant().Contains(w) && !placementschildren.Contains(k))
                                placementschildren.Add(k);
                        }
                    }
                    foreach (var kid in placementschildren)
                    {
                        placementsresult.Add(kid);
                        var currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var parent = GetParent(currentParentCode, placements);
                            if (!placementsresult.Contains(parent)) placementsresult.Add(parent);
                            currentParentCode = parent.ParentCode;
                        }
                    }
                    placements = placementsresult;
                }

                var placementdata = new List<Models.TreeNode>();
                var parents = placements.Where(o => o.ParentCode == 0 || o.ParentCode == -99).OrderBy(o => o.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildren(placements, p.Code),
                        CanBeParent = p.Flagged,
                        parenttitle = p.ParentName
                    };
                    placementdata.Add(parent);
                }
                return Ok(placementdata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("search")]
        public ActionResult<List<Models.TreeNode>> GetPublication2([FromBody] Models.ConfigurationcSearch data)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Placement]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var placements = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    placements.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["IsParent"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var placementschildren = new List<Models.GenericTree>();
                    var placementsresult = new List<Models.GenericTree>();
                    foreach (var word in data.Name.Split(','))
                    {
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var k in placements)
                        {
                            if (k.Name.ToLowerInvariant().Contains(w) && !placementschildren.Contains(k))
                                placementschildren.Add(k);
                        }
                    }
                    foreach (var kid in placementschildren)
                    {
                        placementsresult.Add(kid);
                        var currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var parent = GetParent(currentParentCode, placements);
                            if (!placementsresult.Contains(parent)) placementsresult.Add(parent);
                            currentParentCode = parent.ParentCode;
                        }
                    }
                    placements = placementsresult;
                }

                var placementdata = new List<Models.TreeNode>();
                var parents = placements.Where(o => o.ParentCode == 0 || o.ParentCode == -99 || o.Flagged).OrderBy(o => o.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildren(placements, p.Code),
                        CanBeParent = p.Flagged,
                        parenttitle = p.ParentName
                    };
                    if (p.ParentCode == 0)
                        placementdata.Add(parent);
                }
                return Ok(placementdata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/publicationlist/{codesite:int}/{codeproperty:int?}")]
        public ActionResult<List<Models.GenericList>> GetPublicationList(int codesite, int codeproperty = -1)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Placement]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var placements = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    placements.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["Name"]),
                        IsParent = GetBool(r["IsParent"]) 
                    });
                }
                return Ok(placements);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("sharing/{codeplacement:int}")]
        public ActionResult<List<Models.TreeNode>> GetPublicationSharing(int codeplacement)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_SharingPlacement]";
                cmd.Parameters.Add("@CodePlacement", SqlDbType.Int).Value = codeplacement;
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
        public ActionResult<Models.ResponseCallBack> SavePublication([FromBody] Models.GenericData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing)
                {
                    if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                }
                var codeSiteList = Common.Join(arrSharing, string.Empty, string.Empty, ",");
                if (string.IsNullOrWhiteSpace(codeSiteList) && data.Info.Global)
                    codeSiteList = data.Info.CodeSite;

                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[API_Manage_Publication_Update]";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@CodeParent", SqlDbType.Int).Value = data.Info.ParentCode;
                cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 4000).Value = codeSiteList;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                cmd.Parameters.Add("@IsParent", SqlDbType.Bit).Value = data.Info.IsParent;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cmd.Parameters["@Code"].Direction = ParameterDirection.InputOutput;
                cn.Open();
                _trans = cn.BeginTransaction();
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0)
                {
                    _trans.Rollback();
                    response.Code = resultCode; response.Status = false; response.Message = "Save publication failed"; response.ReturnValue = string.Empty;
                    return StatusCode(500, response);
                }
                response.Code = 0; response.Message = "OK"; response.ReturnValue = GetInt(cmd.Parameters["@Code"].Value, -1); response.Status = true;
                _trans.Commit();
                return Ok(response);
            }
            catch (Exception)
            {
                try { _trans?.Rollback(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode; response.Status = false; response.Message = "Save publication failed";
                return StatusCode(500, response);
            }
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeletePublication([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrPublicationCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrPublicationCodes.Contains(c.Code)) arrPublicationCodes.Add(c.Code);
                }
                var codePublicationList = Common.Join(arrPublicationCodes, string.Empty, string.Empty, ",");
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codePublicationList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "PLACEMENT";
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
                    response.Code = resultCode; response.Status = false; response.Message = "Delete publication failed"; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
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

        private static Models.GenericTree GetParent(int parentcode, List<Models.GenericTree> keywords)
            => keywords.Single(obj => obj.Code == parentcode);

        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> keyworddata, int code)
        {
            var children = new List<Models.TreeNode>();
            if (keyworddata != null)
            {
                var kids = keyworddata.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.TreeNode
                    {
                        title = k.Name,
                        CanBeParent = k.Flagged,
                        key = k.Code,
                        icon = false,
                        children = CreateChildren(keyworddata, k.Code),
                        select = k.Flagged,
                        parenttitle = k.ParentName
                    };
                    children.Add(child);
                }
            }
            return children;
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
