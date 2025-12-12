using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static EgsData.modGlobalDeclarations; // ConnectionString, DebugEnabled, GroupLevel
using static EgsData.modFunctions;         // GetInt, GetStr, GetBool, Common.ReplaceSpecialCharacters, Common.SendEmail

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class BrandController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpGet("/api/brands/{codeliste:int}/{codetrans:int}")]
        public ActionResult<List<Models.GenericList>> GetBrandByCode(int codeliste, int codetrans)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[GET_ListeIngredientsBrandList]";
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
                }

                var ingredientbrands = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    ingredientbrands.Add(new Models.GenericList
                    {
                        Code = GetInt(r["CodeBrand"]),
                        Value = GetStr(r["NameBrand"]) 
                    });
                }
                return Ok(ingredientbrands.ToList());
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/brands/{codesite:int}/{codetrans:int}/{activeonly:bool?}/{tree?}/{name?}")]
        public ActionResult<object> GetBrandBySite(int codesite, int codetrans, bool activeonly = true, bool tree = true, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(name) || name == "null" || name == "undefined") name = null;
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[GET_BRANDCODENAME]";
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                        cmd.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = activeonly;
                        cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = (object?)name ?? DBNull.Value;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
                }

                var brands = new List<Models.Brand>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    brands.Add(new Models.Brand
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = false
                    });
                }

                if (tree)
                {
                    var branddata = new List<Models.BrandTreeNode>();
                    var parents = brands.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                    foreach (var p in parents)
                    {
                        var parent = new Models.BrandTreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateChildren1(brands, p.Code),
                            select = p.Flagged,
                            parenttitle = p.ParentName
                        };
                        branddata.Add(parent);
                    }
                    return Ok(branddata);
                }
                else
                {
                    return Ok(brands);
                }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/brand/tree/{codesite:int}/{codetrans:int}/{codebrands}/{codeliste:int}/{brandclass:int}")]
        public ActionResult<List<Models.BrandTreeNode>> GetBrandTree(int codesite, int codetrans, string codebrands, int codeliste, int brandclass)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_Brands]";
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                        cmd.Parameters.Add("@CodeBrands", SqlDbType.VarChar, 2000).Value = codebrands;
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
                }

                var brands = new List<Models.Brand>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    if (GetInt(r["IsIngredientbrand"]) == brandclass)
                    {
                        brands.Add(new Models.Brand
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["Name"]),
                            ParentCode = GetInt(r["ParentCode"]),
                            ParentName = GetStr(r["ParentName"]),
                            Flagged = GetBool(r["Flagged"]),
                            Note = GetInt(r["IsIngredientbrand"]),
                            Classification = GetInt(r["BrandClassification"]) 
                        });
                    }
                }

                var branddata = new List<Models.BrandTreeNode>();
                var parents = brands.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.BrandTreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildren1(brands, p.Code),
                        select = p.Flagged,
                        parenttitle = p.ParentName,
                        note = p.Note,
                        classification = p.Classification
                    };
                    branddata.Add(parent);
                }
                return Ok(branddata);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        private static List<Models.BrandTreeNode> CreateChildren1(List<Models.Brand> keyworddata, int code)
        {
            var children = new List<Models.BrandTreeNode>();
            if (keyworddata != null)
            {
                var kids = keyworddata.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.BrandTreeNode
                    {
                        title = k.Name,
                        key = k.Code,
                        icon = false,
                        children = CreateChildren1(keyworddata, k.Code),
                        select = k.Flagged,
                        parenttitle = k.ParentName,
                        classification = k.Classification,
                        note = k.Note
                    };
                    children.Add(child);
                }
            }
            return children;
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

        [HttpGet("/api/brand/{codesite:int}/{codeuser:int}/{name?}")]
        public ActionResult<List<Models.TreeNode>> GetBrand(int codesite, int codeuser, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_Brands]";
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = -1;
                        cmd.Parameters.Add("@CodeBrands", SqlDbType.NVarChar).Value = "";
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = -1;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
                }

                var brands = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    brands.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Global = GetBool(r["Global"]),
                        CanBeParent = GetBool(r["IsCanBeParent"]),
                        Type = 0,
                        Note = ""
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var brandschildren = new List<Models.GenericTree>();
                    var brandsresult = new List<Models.GenericTree>();
                    var arrNames = name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        foreach (var k in brands)
                        {
                            if (k.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                            {
                                if (!brandschildren.Contains(k)) brandschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in brandschildren)
                    {
                        brandsresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, brands);
                            if (!brandsresult.Contains(currentParent)) brandsresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    brands = brandsresult;
                }

                var branddata = new List<Models.TreeNode>();
                var parents = brands.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildren1(brands, p.Code, p.Name),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        note = p.Note,
                        CanBeParent = p.CanBeParent,
                        Global = p.Global
                    };
                    branddata.Add(parent);
                }

                return Ok(branddata);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        private static List<Models.TreeNode> CreateChildren1(List<Models.GenericTree> keyworddata, int code, string parentname)
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
                        key = k.Code,
                        icon = false,
                        children = CreateChildren1(keyworddata, k.Code, k.Name),
                        select = k.Flagged,
                        selected = k.Flagged,
                        parenttitle = parentname,
                        ParentCode = k.ParentCode,
                        note = k.Note,
                        CanBeParent = k.CanBeParent
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        private static Models.GenericTree GetParent(int parentcode, List<Models.GenericTree> keywords)
        {
            return keywords.Single(obj => obj.Code == parentcode);
        }

        [HttpGet("/api/brandlist/{codesite:int}/{code:int}/{codetrans:int}/{parentOnly:bool?}")]
        public ActionResult<List<Models.GenericList>> GetBrandParent(int codesite, int code, int codetrans, bool parentOnly = false)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_Brands]";
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cmd.Parameters.Add("@CodeBrands", SqlDbType.Int).Value = code;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = -1;
                        cmd.Parameters.Add("@ParentOnly", SqlDbType.Bit).Value = parentOnly;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
                }

                var cookbooks = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cookbooks.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["Name"]) 
                    });
                }
                return Ok(cookbooks);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/brandkiosk/{codesite:int}/{codebrand:int}")]
        public ActionResult<List<Models.TreeNode>> GetBrandKiosk(int codesite, int codebrand)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "BRANDSITE";
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
                }

                var keywords = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    keywords.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = 0,
                        ParentName = ""
                    });
                }

                var keywordsdata = new List<Models.TreeNode>();
                var parents = keywords.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (!keywordsdata.Any(obj => obj.key == p.Code))
                    {
                        var parent = new Models.TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            select = CheckifSelected(codebrand, p.Code),
                            children = CreateChildren1(keywords, codebrand, p.Name),
                            parenttitle = p.ParentName
                        };
                        keywordsdata.Add(parent);
                    }
                }
                return Ok(keywordsdata);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        private static bool CheckifSelected(int codebrand, int codebrandsite)
        {
            bool isSelected = false;
            var sb = new StringBuilder();
            sb.Append("Select @isSelected = CASE WHEN (COUNT(*)>0)  THEN 1 ELSE 0 END FROM BrandToBrandSite WHERE Brand =  @codebrand AND BrandSite = @codebrandsite");
            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(sb.ToString(), cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@codebrand", SqlDbType.Int).Value = codebrand;
                cmd.Parameters.Add("@codebrandsite", SqlDbType.Int).Value = codebrandsite;
                var sel = cmd.Parameters.Add("@isSelected", SqlDbType.Bit);
                sel.Direction = ParameterDirection.Output;
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                isSelected = Convert.ToBoolean(sel.Value);
            }
            catch { }
            return isSelected;
        }

        [HttpPost("api/brand")]
        public ActionResult<Models.ResponseCallBack> SaveBrand([FromBody] Models.BrandData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode;
            SqlTransaction? _trans = null;
            string strName = data.Info.Name
                .Replace("[r]", "®")
                .Replace("[tm]", "")
                .Replace("[c]", "©")
                .Replace("[R]", "®")
                .Replace("[TM]", "")
                .Replace("[C]", "©");

            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                using (var cmd = new SqlCommand())
                {
                    var arrSharing = new ArrayList();
                    foreach (var sh in data.Sharing)
                        if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                    string codeSiteList = Common.Join(arrSharing, "(", ")", ",");

                    var arrmergelist = new ArrayList();
                    foreach (var sh in data.MergeList)
                        if (!arrmergelist.Contains(sh)) arrmergelist.Add(sh);
                    string mergelist = Common.Join(arrmergelist, "(", ")", ",");

                    using (var cn = new SqlConnection(ConnectionString))
                    {
                        try
                        {
                            cmd.Connection = cn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "[MANAGE_BRANDUPDATE]";
                            cmd.Parameters.Clear();

                            var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
                            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
                            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = strName;
                            cmd.Parameters.Add("@ListeType", SqlDbType.Int).Value = 2;
                            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                            cmd.Parameters.Add("@CodeSiteList", SqlDbType.VarChar, 8000).Value = codeSiteList;
                            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Info.CodeUser;
                            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Info.CodeSite;
                            cmd.Parameters.Add("@CodeParent", SqlDbType.Int).Value = data.Info.ParentCode;
                            cmd.Parameters.Add("@IsCanBeParent", SqlDbType.Bit).Value = data.Info.CanBeParent;
                            cmd.Parameters.Add("@MergeList", SqlDbType.NVarChar).Value = mergelist;
                            retval.Direction = ParameterDirection.ReturnValue;
                            cmd.Parameters["@Code"].Direction = ParameterDirection.InputOutput;

                            cn.Open();
                            _trans = cn.BeginTransaction();
                            cmd.Transaction = _trans;
                            cmd.ExecuteNonQuery();
                            int codeBrand = GetInt(cmd.Parameters["@Code"].Value, -1);
                            resultCode = GetInt(retval.Value, -1);
                            if (resultCode != 0)
                                throw new DatabaseException($"[{resultCode}] Save brand failed");

                            if (codeBrand != -1)
                            {
                                foreach (var t in data.Translation)
                                {
                                    cmd.Connection = cn;
                                    cmd.CommandText = "sp_EgswItemTranslationUpdate";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codeBrand;
                                    cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = t.Name;
                                    cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                                    cmd.Parameters.Add("@tntListType", SqlDbType.Int).Value = 14;
                                    cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Profile.Code;
                                    var tr = cmd.Parameters.Add("@retval", SqlDbType.Int);
                                    tr.Direction = ParameterDirection.ReturnValue;
                                    cmd.ExecuteNonQuery();
                                    resultCode = GetInt(tr.Value, -1);
                                    if (resultCode != 0)
                                        throw new DatabaseException($"[{resultCode}] Update brand translation failed");
                                }

                                foreach (var kiosk in data.KioskList)
                                {
                                    cmd.Connection = cn;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "DELETE_BrandBrandSite";
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@Brand", SqlDbType.Int, 4).Value = codeBrand;
                                    cmd.Parameters.Add("@BrandSite", SqlDbType.Int, 4).Value = kiosk.Code;
                                    cmd.ExecuteNonQuery();

                                    cmd.Connection = cn;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "UPDATE_BrandBrandSite";
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@Brand", SqlDbType.Int, 4).Value = codeBrand;
                                    cmd.Parameters.Add("@BrandSite", SqlDbType.Int, 4).Value = kiosk.Code;
                                    cmd.ExecuteNonQuery();
                                }

                                if (data.KioskList.Count == 0)
                                {
                                    cmd.Connection = cn;
                                    cmd.CommandText = "DELETE FROM BrandToBrandSite WHERE Brand=" + codeBrand;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Clear();
                                    cmd.ExecuteNonQuery();
                                }

                                _trans.Commit();
                            }

                            if (data.ActionType == 5 && data.MergeList.Count > 0)
                            {
                                var arrSites = new ArrayList();
                                foreach (var s in data.MergeList)
                                    if (!arrSites.Contains(s)) arrSites.Add(s);
                                string SiteList = Common.Join(arrSites, "(", ")", ",");
                                var sql = new StringBuilder();

                                sql.Append("Delete from EgswSharing WHERE Code IN" + SiteList + "AND CodeEgswTable=18");
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.ExecuteNonQuery();

                                sql.Clear();
                                sql.Append("UPDATE EgswBrand SET Parent=@Code where Parent IN " + SiteList);
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = GetInt(cmd.Parameters["@Code"].Value, -1);
                                cmd.ExecuteNonQuery();

                                sql.Clear();
                                sql.Append("INSERT INTO EgswSharing ");
                                sql.Append("SELECT @Code,0,CodeUserSharedTo,1,18,0,1,0");
                                sql.Append("FROM EgswSharing ");
                                sql.Append("WHERE Code=@Code AND CodeEgswTable=18 AND [Status]=1 AND [Type] IN (1,5) ");
                                sql.Append("    AND CodeUserSharedTo NOT IN (");
                                sql.Append("    SELECT DISTINCT CodeUserSharedTo ");
                                sql.Append("    FROM EgswSharing  ");
                                sql.Append("    WHERE Code IN " + SiteList + " AND CodeEgswTable=18 AND [Status]=1 AND [Type] IN (1,5)");
                                sql.Append(") ");
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = GetInt(cmd.Parameters["@Code"].Value, -1);
                                cmd.ExecuteNonQuery();

                                sql.Clear();
                                sql.Append("UPDATE EgswListe SET  Brand = " + GetInt(cmd.Parameters["@Code"].Value, -1) + " WHERE Brand IN " + mergelist);
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.ExecuteNonQuery();

                                sql.Clear();
                                sql.Append("UPDATE EgswBrand SET  Parent = " + data.Info.ParentCode + " WHERE Code IN " + mergelist);
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.ExecuteNonQuery();

                                sql.Clear();
                                sql.Append("DELETE RecipeBrand WHERE CodeListe IN ");
                                sql.Append(" (select CodeListe from RecipeBrand where brand in " + mergelist + " group by codeliste having count(codeliste) > 1)");
                                sql.Append(" AND Brand  <> (select min(brand) from RecipeBrand where brand in " + mergelist + "\tgroup by codeliste  having count(codeliste) > 1 )");
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.ExecuteNonQuery();

                                sql.Clear();
                                sql.Append("UPDATE RecipeBrand SET  Brand = " + GetInt(cmd.Parameters["@Code"].Value, -1) + " WHERE Brand IN " + mergelist);
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.ExecuteNonQuery();

                                sql.Clear();
                                sql.Append("DELETE FROM BrandToBrandSite WHERE BRAND IN " + mergelist);
                                cmd.CommandText = sql.ToString();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = "API_DELETE_Generic";
                                cmd.CommandType = CommandType.StoredProcedure;
                                foreach (var code in arrSites.Cast<int>())
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = GetInt(code);
                                    cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWBRAND";
                                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                                    cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = false;
                                    cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                                    var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                                    ret.Direction = ParameterDirection.ReturnValue;
                                    cmd.ExecuteNonQuery();
                                    resultCode = GetInt(ret.Value, -1);
                                    if (resultCode != 0)
                                        throw new DatabaseException($"[{resultCode}] Delete merged brand failed");
                                }
                            }

                            response.Code = 0;
                            response.Message = "OK";
                            response.ReturnValue = GetInt(cmd.Parameters["@Code"].Value, -1);
                            response.Status = true;
                        }
                        catch (DatabaseException ex)
                        {
                            Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                            try { _trans?.Rollback(); (_trans as IDisposable)?.Dispose(); } catch { }
                            if (resultCode == 0) resultCode = 500;
                            response.Code = resultCode;
                            response.Status = false;
                            response.Message = "Save brand failed";
                            Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Brand");
                        }
                        finally
                        {
                            cn.Close(); (cn as IDisposable)?.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Brand");
            }
            return Ok(response);
        }

        [HttpPost("api/brand/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteBrand([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode;
            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        var arrCategoryCodes = new ArrayList();
                        foreach (var c in data.CodeList)
                            if (!arrCategoryCodes.Contains(c.Code)) arrCategoryCodes.Add(c.Code);
                        string codeCategoryList = Common.Join(arrCategoryCodes, "", "", ",");

                        cmd.Connection = cn;
                        cmd.CommandText = "API_DELETE_Generic";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeCategoryList;
                        cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWBRAND";
                        cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                        cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                        var skipList = cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000);
                        skipList.Direction = ParameterDirection.Output;
                        var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                        ret.Direction = ParameterDirection.ReturnValue;

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(ret.Value, -1);
                        if (resultCode != 0)
                            throw new DatabaseException($"[{resultCode}] Delete brand failed");

                        response.Code = 0;
                        response.Message = "OK";
                        response.ReturnValue = GetStr(skipList.Value);
                        response.Status = true;
                    }
                    catch (DatabaseException ex)
                    {
                        Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                        if (resultCode == 0) resultCode = 500;
                        response.Code = resultCode;
                        response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                        response.Status = false;
                        response.Message = "Delete brand failed";
                        Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Brand");
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
                }
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Brand");
            }
            return Ok(response);
        }
    }
}
