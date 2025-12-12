using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class SharingController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("/api/sharing/{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}")]
        public ActionResult<List<Models.TreeNode>> GetSharing(int codesite, int codetrans, int type, int tree, int codeliste)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Sharing]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
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
                        Type = GetInt(r["Type"]) 
                    });
                }

                var sharingdata = new List<Models.TreeNode>();
                var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        unselectable = p.Code == codesite,
                        addClass = p.Code == codesite ? "main" : null,
                        children = CreateChildren(sharings, p.Code, codesite),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        // groupLevel = GroupLevel.Property // keep default if enum not in scope
                    };
                    sharingdata.Add(parent);
                }

                return Ok(sharingdata);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> sharingdata, int code, int codesite)
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
                        unselectable = (k.Code == codesite),
                        addClass = (k.Code == codesite) ? "main" : null,
                        select = (k.Code == codesite) ? true : k.Flagged,
                        selected = (k.Code == codesite) ? true : k.Flagged,
                        children = null,
                        parenttitle = k.ParentName,
                        // groupLevel = GroupLevel.Site // keep default if enum not in scope
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        // Helpers
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
    }
}
