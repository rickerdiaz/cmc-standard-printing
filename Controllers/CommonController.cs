using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("autonumber/{codesite:int}/{codeuser:int}/{type:int}/{category:int?}")]
        public ActionResult<Models.ResponseCallBack> GetAutoNumber(int codesite, int codeuser, int type, int category = -1)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                string strNumber;
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_AutoNumber]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = category;
                var pNumber = cmd.Parameters.Add("@Number", SqlDbType.VarChar, 50);
                pNumber.Direction = ParameterDirection.Output;
                var pErr = cmd.Parameters.Add("@ERR", SqlDbType.Int);
                pErr.Direction = ParameterDirection.ReturnValue;
                cn.Open();
                cmd.ExecuteNonQuery();
                strNumber = Convert.ToString(pNumber.Value) ?? string.Empty;
                resultCode = Convert.ToInt32(pErr.Value);
                if (resultCode != 0) throw new Exception($"[{resultCode}] Get Autonumber failed");
                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = strNumber;
                response.Status = true;
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Get Autonumber failed";
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        [HttpGet("sharing/{codeegstable:int}/{codesite:int}/{code:int}")]
        public ActionResult<List<Models.TreeNode>> GetSharing(int codeegstable, int codesite, int code)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "[dbo].[API_GET_SharingAll]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = codeegstable;
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
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildrenSharing(sharings, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        groupLevel = Models.GroupLevel.Property
                    };
                    if (parent.children.Count > 0) sharingdata.Add(parent);
                }

                return Ok(sharingdata);
            }
            catch (ArgumentException)
            {
                return Problem(title: $"Request failed ({codeegstable})", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: $"Request failed ({codeegstable})", statusCode: 500);
            }
        }

        [HttpGet("translation/{codeegstable:int}/{codesite:int}/{code:int}")]
        public ActionResult<List<Models.GenericTranslation>> GetTranslation(int codeegstable, int codesite, int code)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "API_GET_TranslationAll";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = codeegstable;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var translations = new List<Models.GenericTranslation>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    translations.Add(new Models.GenericTranslation
                    {
                        CodeTrans = GetInt(r["CodeTrans"]),
                        TranslationName = GetStr(r["TranslationName"]),
                        Name = GetStr(r["Name"]) 
                    });
                }
                return Ok(translations);
            }
            catch (ArgumentException)
            {
                return Problem(title: $"Request failed ({codeegstable})", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: $"Request failed ({codeegstable})", statusCode: 500);
            }
        }

        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> sharings, int code)
        {
            var children = new List<Models.TreeNode>();
            var kids = sharings.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new Models.TreeNode
                {
                    title = k.Name,
                    key = k.Code,
                    icon = false,
                    children = null,
                    select = k.Flagged,
                    selected = k.Flagged,
                    parenttitle = k.ParentName,
                    groupLevel = Models.GroupLevel.Property,
                    note = k.Global
                };
                children.Add(child);
            }
            return children;
        }

        private static int GetInt(object? value, int fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (int.TryParse(Convert.ToString(value), out var i)) return i;
            try { return Convert.ToInt32(value); } catch { return fallback; }
        }
        private static string GetStr(object? value)
        {
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
        }
        private static bool GetBool(object? value)
        {
            if (value == null || value == DBNull.Value) return false;
            if (bool.TryParse(Convert.ToString(value), out var b)) return b;
            try { return Convert.ToInt32(value) != 0; } catch { return false; }
        }
    }

    // Placeholder models - replace with actual
    namespace Models
    {
        public class ResponseCallBack { public int Code { get; set; } public string Message { get; set; } = string.Empty; public object? ReturnValue { get; set; } public bool Status { get; set; } public List<param>? Parameters { get; set; } }
        public class param { public string name { get; set; } = string.Empty; public string value { get; set; } = string.Empty; }
        public class TreeNode { public string title { get; set; } = string.Empty; public int key { get; set; } public bool icon { get; set; } public List<TreeNode>? children { get; set; } public bool select { get; set; } public bool selected { get; set; } public string parenttitle { get; set; } = string.Empty; public GroupLevel groupLevel { get; set; } public object? note { get; set; } }
        public class GenericTree { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public string ParentName { get; set; } = string.Empty; public bool Flagged { get; set; } public int Type { get; set; } public bool Global { get; set; } }
        public class GenericTranslation { public int CodeTrans { get; set; } public string TranslationName { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; }
        public enum GroupLevel { Property, Site }
    }
}
