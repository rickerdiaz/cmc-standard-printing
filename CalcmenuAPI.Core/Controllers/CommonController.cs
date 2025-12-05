using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static EgsData.modGlobalDeclarations; // ConnectionString, GroupLevel
using static EgsData.modFunctions;         // GetInt, GetStr, GetBool

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class CommonController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpGet("/api/autonumber/{codesite:int}/{codeuser:int}/{type:int}/{category:int?}")]
        public ActionResult<Models.ResponseCallBack> GetAutoNumber(int codesite, int codeuser, int type, int category = -1)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                string strNumber = string.Empty;
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_GET_AutoNumber]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = category;
                    var number = cmd.Parameters.Add("@Number", SqlDbType.VarChar, 50);
                    number.Direction = ParameterDirection.Output;
                    var err = cmd.Parameters.Add("@ERR", SqlDbType.Int);
                    err.Direction = ParameterDirection.ReturnValue;
                    cn.Open();
                    cmd.ExecuteNonQuery();

                    strNumber = GetStr(number.Value);
                    resultCode = GetInt(err.Value);
                    if (resultCode != 0)
                        throw new DatabaseException($"[{resultCode}] Get Autonumber failed");

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = strNumber;
                    response.Status = true;
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
            }
            return Ok(response);
        }

        [HttpGet("/api/common/sharing/{codeegstable:int}/{codesite:int}/{code:int}")]
        public ActionResult<List<Models.TreeNode>> GetSharing(int codeegstable, int codesite, int code)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "[dbo].[API_GET_SharingAll]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = codeegstable;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

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
                        groupLevel = GroupLevel.Property
                    };
                    if (parent.children != null && parent.children.Count > 0) sharingdata.Add(parent);
                }

                return Ok(sharingdata);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + "(" + codeegstable + ")" + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + "(" + codeegstable + ")" + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/common/translation/{codeegstable:int}/{codesite:int}/{code:int}")]
        public ActionResult<List<Models.GenericTranslation>> GetTranslation(int codeegstable, int codesite, int code)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "API_GET_TranslationAll";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                    cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = codeegstable;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as System.IDisposable)?.Dispose(); }

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

                return Ok(translations.ToList());
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + "(" + codeegstable + ")" + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + "(" + codeegstable + ")" + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
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
                        selected = k.Flagged,
                        parenttitle = k.ParentName,
                        groupLevel = GroupLevel.Site,
                        note = k.Global
                    };
                    children.Add(child);
                }
            }
            return children;
        }
    }
}
