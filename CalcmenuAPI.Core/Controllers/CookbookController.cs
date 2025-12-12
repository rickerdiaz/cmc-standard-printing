using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using Microsoft.AspNetCore.Mvc;
using static EgsData.modGlobalDeclarations; // ConnectionString, DebugEnabled, GroupLevel
using static EgsData.modFunctions;         // GetInt, GetStr, GetBool, Common.Join, Common.MapPath, Common.ReplaceSpecialCharacters, Common.SendEmail

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class CookbookController : ControllerBase
    {
        private string m_PictureNames = string.Empty;
        private string m_TempPictureNames = string.Empty;
        private string[] arrPictures = Array.Empty<string>();

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpGet("/api/cookbook/{codeliste:int}")]
        public ActionResult<Models.CookbookData> GetCookbookById(int codeliste)
        {
            var data = new Models.CookbookData();
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_GET_ProjectInfo]";
                    cmd.Parameters.Add("@CodeProject", SqlDbType.Int).Value = codeliste;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                if (ds != null && ds.Tables.Count == 3)
                {
                    var tblInfo = ds.Tables[0];
                    var tblSites = ds.Tables[1];
                    var tblUser = ds.Tables[2];

                    foreach (DataRow dr in tblInfo.Rows)
                    {
                        data.Info = new Models.Cookbook
                        {
                            Code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            ParentCode = GetInt(dr["ParentCode"]),
                            CodeSite = GetInt(dr["CodeSite"]),
                            CodeOwner = GetInt(dr["CodeOwner"]),
                            Global = GetBool(dr["Global"]),
                            CanBeParent = GetBool(dr["CanBeParent"]),
                            Picture = GetStr(dr["Picture"]),
                            hasPicture = GetBool(dr["hasPicture"]) 
                        };
                        if (string.IsNullOrWhiteSpace(data.Info.Picture)) data.Info.Picture = "default.png";
                    }

                    if (tblSites.Rows.Count > 0)
                    {
                        data.Sharing = new List<Models.GenericList>();
                        foreach (DataRow dr in tblSites.Rows)
                            data.Sharing.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Name"]) });
                    }

                    if (tblUser.Rows.Count > 0)
                    {
                        data.Users = new List<Models.GenericList>();
                        foreach (DataRow dr in tblUser.Rows)
                            data.Users.Add(new Models.GenericList { Code = GetInt(dr["CodeUser"]), Value = GetStr(dr["IsAssigned"]), Name = GetStr(dr["Name"]), Value2 = GetInt(dr["CodeSite"]) });
                    }
                }
                return Ok(data);
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

        [HttpGet("/api/cookbook/{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.TreeNode>> GetCookbookByName(int codesite, int codetrans, int type, int tree, int codeliste, int codeproperty = -1, string? name = "")
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
                    cmd.CommandText = "[dbo].[API_GET_Projects]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
                    cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var projects = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    projects.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Picture = GetStr(r["ParentName"]),
                        hasPicture = GetBool(r["hasPicture"]),
                        Flagged = GetBool(r["Flagged"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var projectschildren = new List<Models.GenericTree>();
                    var projectsresult = new List<Models.GenericTree>();
                    var arrNames = name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        foreach (var k in projects)
                        {
                            if (k.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                            {
                                if (!projectschildren.Contains(k)) projectschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in projectschildren)
                    {
                        projectsresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, projects);
                            projectsresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    projects = projectsresult;
                }

                var projectdata = new List<Models.TreeNode>();
                var parents = projects.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (!projectdata.Any(obj => obj.key == p.Code))
                    {
                        var parent = new Models.TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateChildren(projects, p.Code),
                            select = p.Flagged,
                            selected = p.Flagged,
                            parenttitle = p.ParentName
                        };
                        projectdata.Add(parent);
                    }
                }
                return Ok(projectdata);
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

        [HttpGet("/api/cookbooklist/{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{name?}")]
        public ActionResult<Models.ResponseCookBook> GetCookbookAll(int codesite, int codetrans, int type, int tree, int codeliste, string? name = "", int skip = 0, int take = 10)
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
                    cmd.CommandText = "[dbo].[API_GET_ProjectList]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var projects = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    projects.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"] + " (" + GetStr(r["RecipeCount"]) + ")"),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var projectschildren = new List<Models.GenericTree>();
                    var projectsresult = new List<Models.GenericTree>();
                    var arrNames = name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        foreach (var k in projects)
                        {
                            if (k.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                            {
                                if (!projectschildren.Contains(k)) projectschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in projectschildren)
                    {
                        projectsresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, projects);
                            projectsresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    projects = projectsresult;
                }

                var projectdata = new List<Models.TreeNode>();
                var parents = projects.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (!projectdata.Any(obj => obj.key == p.Code))
                    {
                        var parent = new Models.TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateChildren(projects, p.Code),
                            select = p.Flagged,
                            selected = p.Flagged,
                            parenttitle = p.ParentName
                        };
                        projectdata.Add(parent);
                    }
                }

                int totalCount = projectdata.Count;
                take = take > totalCount + 1 ? totalCount : take;
                take = take <= 0 ? 1 : take;
                int totalPages = (int)Math.Ceiling((double)totalCount / take);
                skip = skip > totalPages ? totalPages : skip;
                skip = skip < 1 ? 0 : skip;

                return Ok(new Models.ResponseCookBook(projectdata.Skip(take * skip).Take(take).ToList(), totalCount));
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

        [HttpGet("/api/cookbook/list/{codesite:int}/{codetrans:int}/{type:int}/{codeuser:int}/{codekey:int}/{name?}")]
        public ActionResult<Models.ResponseTreeGeneric> GetCookbookListAll(int codesite, int codetrans, int type, int codeuser, int codekey, string? name = "", int skip = 0, int take = 10)
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
                    cmd.CommandText = "[dbo].[API_GET_ProjectList]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeKey", SqlDbType.Int).Value = codekey;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var projects = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    projects.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"] + " (" + GetStr(r["RecipeCount"]) + ")"),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    name = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(name));
                    var projectschildren = new List<Models.GenericTree>();
                    var projectsresult = new List<Models.GenericTree>();
                    var arrNames = name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        foreach (var k in projects)
                        {
                            if (k.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                            {
                                if (!projectschildren.Contains(k)) projectschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in projectschildren)
                    {
                        projectsresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, projects);
                            projectsresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    projects = projectsresult;
                }

                var projectdata = new List<Models.GenericTreeNode>();
                var parents = projects.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    if (!projectdata.Any(obj => obj.id == p.Code))
                    {
                        var parent = new Models.GenericTreeNode
                        {
                            title = p.Name,
                            id = p.Code,
                            children = CreateChildrenGeneric(projects, p.Code)
                        };
                        projectdata.Add(parent);
                    }
                }

                int totalCount = projectdata.Count;
                take = take > totalCount + 1 ? totalCount : take;
                take = take <= 0 ? 1 : take;
                int totalPages = (int)Math.Ceiling((double)totalCount / take);
                skip = skip > totalPages ? totalPages : skip;
                skip = skip < 1 ? 0 : skip;

                return Ok(new Models.ResponseTreeGeneric(projectdata.Skip(take * skip).Take(take).ToList(), totalCount));
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

        [HttpGet("/api/cookbookrecipe/{codesite:int}/{codetrans:int}/{type:int}/{codeuser:int}/{codekey:int}/{name?}")]
        public ActionResult<Models.ResponseCookBookRecipe> GetCookbookRecipeByName(int codesite, int codetrans, int type, int codeuser, int codekey, string? name = "", int page = 0, int limit = 10)
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
                    cmd.CommandText = "[dbo].[API_GET_ProjectList]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@CodeKey", SqlDbType.Int).Value = codekey;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var projects = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    projects.Add(new Models.GenericTree { Code = GetInt(r["Code"]), Name = GetStr(r["Name"]) });
                }
                var recipeChild = new List<Models.TreeGridNode>();
                foreach (var p in projects)
                {
                    var projectList = new Models.TreeGridNode { title = p.Name, key = p.Code };
                    recipeChild.Add(projectList);
                }

                ds.Clear();
                using var cmd2 = new SqlCommand();
                using var cn2 = new SqlConnection(ConnectionString);
                try
                {
                    cmd2.Connection = cn2;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.CommandText = "[dbo].[API_GET_ProjectRecipeList]";
                    cmd2.Parameters.Add("@CodeKey", SqlDbType.Int).Value = codekey;
                    cmd2.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd2.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd2.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                    cn2.Open();
                    using var _da2 = new SqlDataAdapter(cmd2);
                    _da2.Fill(ds);
                }
                finally { cn2.Close(); (cn2 as IDisposable)?.Dispose(); }

                var recipe = new List<Models.ProjectRecipe>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    recipe.Add(new Models.ProjectRecipe
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["name"]),
                        Subname = GetStr(r["SubTitle"]),
                        PrimaryBrand = GetStr(r["PrimaryBrand"]),
                        SecondaryBrand = GetStr(r["SecondaryBrand"]),
                        Status = GetStr(r["RecipeStatus"]),
                        Image = GetBool(r["ImageDisplay"]),
                        Nutrition = GetBool(r["DisplayNutrition"]),
                        CheckoutUser = GetInt(r["CheckoutUser"]),
                        Number = GetStr(r["Number"]),
                        Picture = GetStr(r["PictureName"]),
                        isLocked = GetBool(r["IsLocked"]) 
                    });
                }

                foreach (var r in recipe)
                {
                    var recipeList = new Models.TreeGridNode
                    {
                        title = r.Name,
                        key = r.Code,
                        expanded = false,
                        image = r.Image,
                        leaf = true,
                        number = r.Number,
                        nutrition = r.Nutrition,
                        primarybrand = r.PrimaryBrand,
                        secondarybrand = r.SecondaryBrand,
                        status = r.Status,
                        subname = r.Subname,
                        CheckoutUser = r.CheckoutUser,
                        Picture = r.Picture,
                        IsLocked = r.isLocked
                    };
                    recipeChild.Add(recipeList);
                }

                int totalCount = recipeChild.Count;
                limit = limit > totalCount + 1 ? totalCount : limit;
                limit = limit <= 0 ? 1 : limit;
                int totalPages = (int)Math.Ceiling((double)totalCount / limit);
                page = page > totalPages ? totalPages : page;
                page = page < 1 ? 0 : page;
                return Ok(new Models.ResponseCookBookRecipe(recipeChild.Skip(limit * (page - 1)).Take(limit).ToList(), totalCount));
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

        [HttpGet("/api/cookbook/{codesite:int}/{codeuser:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.TreeNode>> GetCookbook(int codesite, int codeuser, int codeproperty = -1, string? name = "")
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
                    cmd.CommandText = "[dbo].[API_GET_Projects]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = -1;
                    if (codeuser != -1) cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var cookbooks = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cookbooks.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Global = GetBool(r["Global"]),
                        Type = 0,
                        Note = GetStr(r["CodeUser"]) + "|" + GetStr(r["DateUpdated"]) + "|" + GetStr(r["UserName"]) 
                    });
                }

                if (codeuser > 0)
                {
                    var cookbookschildren = new List<Models.GenericTree>();
                    var cookbooksresult = new List<Models.GenericTree>();
                    foreach (var k in cookbooks)
                    {
                        var arrNote = (k.Note ?? string.Empty).Trim().Split('|');
                        if (arrNote.Length > 0)
                        {
                            if (GetInt(arrNote[0]) == codeuser)
                            {
                                if (!cookbookschildren.Contains(k)) cookbookschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in cookbookschildren)
                    {
                        cookbooksresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, cookbooks);
                            if (!cookbooksresult.Contains(currentParent)) cookbooksresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    cookbooks = cookbooksresult;
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var cookbookschildren = new List<Models.GenericTree>();
                    var cookbooksresult = new List<Models.GenericTree>();
                    var arrNames = name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        foreach (var k in cookbooks)
                        {
                            if (k.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                            {
                                if (!cookbookschildren.Contains(k)) cookbookschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in cookbookschildren)
                    {
                        cookbooksresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, cookbooks);
                            if (!cookbooksresult.Contains(currentParent)) cookbooksresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    cookbooks = cookbooksresult;
                }

                var cookbookdata = new List<Models.TreeNode>();
                var parents = cookbooks.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildren(cookbooks, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        note = p.Note
                    };
                    cookbookdata.Add(parent);
                }

                return Ok(cookbookdata);
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

        [HttpPost("/api/cookbook/search")]
        public ActionResult<List<Models.TreeNode>> GetCookbook2([FromBody] Models.ConfigurationcSearch data)
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
                    cmd.CommandText = "[dbo].[API_GET_Projects]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = -1;
                    if (data.CodeUser != -1) cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var cookbooks = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cookbooks.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Global = GetBool(r["Global"]),
                        Type = 0,
                        Note = GetStr(r["CodeUser"]) + "|" + GetStr(r["DateUpdated"]) + "|" + GetStr(r["UserName"]) 
                    });
                }

                if (data.CodeUser > 0)
                {
                    var cookbookschildren = new List<Models.GenericTree>();
                    var cookbooksresult = new List<Models.GenericTree>();
                    foreach (var k in cookbooks)
                    {
                        var arrNote = (k.Note ?? string.Empty).Trim().Split('|');
                        if (arrNote.Length > 0)
                        {
                            if (GetInt(arrNote[0]) == data.CodeUser)
                            {
                                if (!cookbookschildren.Contains(k)) cookbookschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in cookbookschildren)
                    {
                        cookbooksresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, cookbooks);
                            if (!cookbooksresult.Contains(currentParent)) cookbooksresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    cookbooks = cookbooksresult;
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var cookbookschildren = new List<Models.GenericTree>();
                    var cookbooksresult = new List<Models.GenericTree>();
                    var arrNames = data.Name.Trim().Split(',');
                    foreach (var word in arrNames)
                    {
                        foreach (var k in cookbooks)
                        {
                            if (k.Name.ToLower().Contains(Common.ReplaceSpecialCharacters(word.Trim().ToLower())))
                            {
                                if (!cookbookschildren.Contains(k)) cookbookschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in cookbookschildren)
                    {
                        cookbooksresult.Add(kid);
                        int currentParentCode = kid.ParentCode;
                        while (currentParentCode > 0)
                        {
                            var currentParent = GetParent(currentParentCode, cookbooks);
                            if (!cookbooksresult.Contains(currentParent)) cookbooksresult.Add(currentParent);
                            currentParentCode = currentParent.ParentCode;
                        }
                    }
                    cookbooks = cookbooksresult;
                }

                var cookbookdata = new List<Models.TreeNode>();
                var parents = cookbooks.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildren(cookbooks, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        note = p.Note
                    };
                    cookbookdata.Add(parent);
                }

                return Ok(cookbookdata);
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

        [HttpGet("/api/cookbooklist/{codesite:int}/{code:int}/{parentOnly:bool?}")]
        public ActionResult<List<Models.GenericList>> GetCookbookList(int codesite, int code, bool parentOnly = false)
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
                    cmd.CommandText = "[dbo].[API_GET_Projects]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = -1;
                    cmd.Parameters.Add("@ParentOnly", SqlDbType.Bit).Value = parentOnly;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                var cookbooks = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cookbooks.Add(new Models.GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
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

        [HttpGet("/api/cookbook/sharing/{codeproject:int}")]
        public ActionResult<List<Models.TreeNode>> GetCookbookSharing(int codeproject)
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
                    cmd.CommandText = "[dbo].[API_GET_SharingProject]";
                    cmd.Parameters.Add("@CodeProject", SqlDbType.Int).Value = codeproject;
                    cn.Open();
                    using var _da = new SqlDataAdapter(cmd);
                    _da.Fill(ds);
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

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
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost("/api/cookbook")]
        public ActionResult<Models.ResponseCallBack> SaveCookbook([FromBody] Models.CookbookData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;
            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }));

                if (data == null) throw new ArgumentNullException("cookbook data is empty");
                if (data.Info == null) throw new ArgumentNullException("invalid cookbook data");

                int intCodeProject = GetInt(data.Info.Code);

                using var cmd = new SqlCommand();
                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing)
                    if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                string codeSiteList = Common.Join(arrSharing, "(", ")", ",");
                codeSiteList = Common.Join(arrSharing, "", "", ",");

                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = SP_API_UPDATE_Project;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
                    cmd.Parameters.Add("@Global", SqlDbType.Bit).Value = data.Info.Global;
                    cmd.Parameters.Add("@CodeParent", SqlDbType.Int).Value = data.Info.ParentCode;
                    cmd.Parameters.Add("@CodeOwner", SqlDbType.Int).Value = data.Info.CodeOwner;
                    cmd.Parameters.Add("@CanBeParent", SqlDbType.Bit).Value = data.Info.CanBeParent;
                    cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 400).Value = data.Info.Picture ?? string.Empty;
                    cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 2000).Value = codeSiteList;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                    cmd.Parameters["@Code"].Direction = ParameterDirection.InputOutput;
                    var retOut = cmd.Parameters.Add("@Return", SqlDbType.Int);
                    retOut.Direction = ParameterDirection.Output;

                    cn.Open();
                    _trans = cn.BeginTransaction();
                    cmd.Transaction = _trans;
                    cmd.ExecuteNonQuery();

                    intCodeProject = GetInt(cmd.Parameters["@Code"].Value, -1);
                    resultCode = GetInt(retOut.Value, -1);
                    if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save cookbook data failed");
                    if (intCodeProject <= 0) throw new DatabaseException($"[{resultCode}] Cookbook not created");

                    if (data.Sharing != null)
                    {
                        var sharingList = data.Sharing.Select(row => row.Code).Distinct().ToList();
                        string codeSharedTo = string.Join(",", sharingList);
                        cmd.CommandText = SP_API_UPDATE_Sharing;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Code", SqlDbType.Int).Value = intCodeProject;
                        cmd.Parameters.Add("@CodeOwner", SqlDbType.Int).Value = data.Info.CodeOwner;
                        cmd.Parameters.Add("@CodeSharedToList", SqlDbType.VarChar, 4000).Value = codeSharedTo;
                        cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 152;
                        cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                        var retShare = cmd.Parameters.Add("@Return", SqlDbType.Int);
                        retShare.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(retShare.Value, -1);
                        if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save cookbook sharing failed");
                    }

                    if (data.Users != null)
                    {
                        var userList = data.Users.Where(row => row.Value == "1").Select(row => row.Code).Distinct().ToList();
                        string codeusers = string.Join(",", userList);
                        cmd.CommandText = SP_API_UPDATE_ProjectUser;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeProject", SqlDbType.Int).Value = intCodeProject;
                        cmd.Parameters.Add("@CodeUsers", SqlDbType.VarChar, 4000).Value = codeusers;
                        var retUsers = cmd.Parameters.Add("@Return", SqlDbType.Int);
                        retUsers.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(retUsers.Value, -1);
                        if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save cookbook users failed");
                    }

                    string procpics = (data.Info.Picture ?? string.Empty) + ";";
                    m_PictureNames = procpics;
                    m_TempPictureNames = data.Info.Picture ?? string.Empty;
                    arrPictures = procpics.Split(';');
                    Log.Info("arrayPictures" + arrPictures.ToString());
                    var NewThread = new Thread(SavePictures) { Priority = ThreadPriority.Lowest };
                    NewThread.Start();

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = intCodeProject;
                    response.Status = true;
                    _trans.Commit();
                }
                catch (DatabaseException ex)
                {
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                    try { _trans?.Rollback(); (_trans as IDisposable)?.Dispose(); } catch { }
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.Message = "Save cookbook failed";
                    Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Cookbook");
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, aex.Message, aex.StackTrace ?? string.Empty, "Cookbook");
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Cookbook");
            }
            return Ok(response);
        }

        [HttpPost("/api/cookbook/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteCookbook([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }));

                using var cmd = new SqlCommand();
                var arrProjectCodes = new ArrayList();
                foreach (var c in data.CodeList)
                    if (!arrProjectCodes.Contains(c.Code)) arrProjectCodes.Add(c.Code);
                string codeProjectList = Common.Join(arrProjectCodes, "", "", ",");

                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "API_DELETE_Generic";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeProjectList;
                    cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "PROJECT";
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
                    if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete cookbook failed");

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
                    response.Status = false;
                    response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                    response.Message = "Delete cookbook failed";
                    Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Cookbook");
                }
                finally { cn.Close(); (cn as IDisposable)?.Dispose(); }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, aex.Message, aex.StackTrace ?? string.Empty, "Cookbook");
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                Common.SendEmail(HttpContext?.Request?.Path.ToString() ?? string.Empty, ex.Message, ex.StackTrace ?? string.Empty, "Cookbook");
            }
            return Ok(response);
        }

        private static Models.GenericTree GetParent(int parentcode, List<Models.GenericTree> keywords)
        {
            var k = keywords.Where(obj => obj.Code == parentcode);
            if (k.Any()) return k.First();
            return new Models.GenericTree();
        }

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
                        key = k.Code,
                        icon = false,
                        children = CreateChildren(keyworddata, k.Code),
                        select = k.Flagged,
                        selected = k.Flagged,
                        parenttitle = k.ParentName,
                        note = k.Note
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        private static List<Models.GenericTreeNode> CreateChildrenGeneric(List<Models.GenericTree> keyworddata, int code)
        {
            var children = new List<Models.GenericTreeNode>();
            if (keyworddata != null)
            {
                var kids = keyworddata.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.GenericTreeNode
                    {
                        title = k.Name,
                        id = k.Code,
                        children = CreateChildrenGeneric(keyworddata, k.Code)
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        private static List<Models.TreeGridNode> CreateChildrenRecipe(List<Models.GenericTree> keyworddata, int code)
        {
            var children = new List<Models.TreeGridNode>();
            if (keyworddata != null)
            {
                var kids = keyworddata.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.TreeGridNode();

                    var ds = new DataSet();
                    using var cmd = new SqlCommand();
                    using var cn = new SqlConnection(ConnectionString);
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[GET_ProjectRecipeList]";
                        cmd.Parameters.Add("@CodeKey", SqlDbType.Int).Value = k.Code;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = 1;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally { cn.Close(); (cn as IDisposable)?.Dispose(); }

                    var recipe = new List<Models.ProjectRecipe>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        recipe.Add(new Models.ProjectRecipe
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["name"]),
                            Subname = GetStr(r["SubTitle"]),
                            PrimaryBrand = GetStr(r["PrimaryBrand"]),
                            SecondaryBrand = GetStr(r["SecondaryBrand"]),
                            Status = GetStr(r["RecipeStatus"]),
                            Image = GetBool(r["ImageDisplay"]),
                            Nutrition = GetBool(r["DisplayNutrition"]),
                            CheckoutUser = GetInt(r["CheckoutUser"]),
                            Number = GetStr(r["Number"]) 
                        });
                    }

                    var recipeChild = new List<Models.TreeGridNode>();
                    foreach (var r in recipe)
                    {
                        var recipeList = new Models.TreeGridNode
                        {
                            title = r.Name,
                            key = r.Code,
                            expanded = false,
                            image = r.Image,
                            leaf = true,
                            number = r.Number,
                            nutrition = r.Nutrition,
                            primarybrand = r.PrimaryBrand,
                            secondarybrand = r.SecondaryBrand,
                            status = r.Status,
                            subname = r.Subname,
                            children = recipeChild,
                            parenttitle = k.Name,
                            CheckoutUser = r.CheckoutUser
                        };
                        children.Add(recipeList);
                    }

                    child.title = k.Name;
                    child.key = k.Code;
                    child.note = recipe.Count;
                    child.leaf = false;
                    child.expanded = false;
                    child.children = CreateChildrenRecipe(keyworddata, k.Code);
                    children.Add(child);
                    child.parenttitle = k.ParentName;
                }
            }
            return children;
        }

        private void SavePictures()
        {
            try
            {
                string strTempFolder = TempFolder2;
                if (!string.IsNullOrWhiteSpace(m_PictureNames))
                {
                    var arrPictureNames = m_PictureNames.Trim().Split(';');
                    m_PictureNames = string.Join(";", arrPictures);
                    if (arrPictureNames.Length > 0)
                    {
                        for (int ctr = 0; ctr < arrPictureNames.Length; ctr++)
                        {
                            if (!string.IsNullOrWhiteSpace(arrPictureNames[ctr]))
                            {
                                string pic = arrPictureNames[ctr];
                                string _source;
                                if (pic.Trim().IndexOf("| DAM", StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    pic = pic.Substring(0, pic.IndexOf("|", StringComparison.Ordinal)).Trim();
                                    _source = DamFolder + pic;
                                    File.Copy(_source, strTempFolder + arrPictures[ctr], true);
                                    pic = arrPictures[ctr];
                                }
                                _source = strTempFolder + pic;
                                if (File.Exists(_source))
                                {
                                    File.Copy(_source, PicOriginalFolder + pic, true);
                                    fctResizeConvertImage(_source, PicNormalFolder + pic, 300, 300, false);
                                    fctResizeConvertImage(_source, PicThumbnailFolder + pic, 200, 200, false);
                                    Log.Info("Picture saved:" + _source);
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(m_TempPictureNames))
                {
                    var arrPictureNames = m_TempPictureNames.Trim().Split(';');
                    if (arrPictureNames.Length > 0)
                    {
                        foreach (var pic in arrPictureNames)
                        {
                            if (!string.IsNullOrWhiteSpace(pic))
                            {
                                if (pic.Trim().LastIndexOf("| DAM", StringComparison.OrdinalIgnoreCase) == -1)
                                {
                                    string _source = strTempFolder + pic;
                                    if (File.Exists(_source)) File.Delete(_source);

                                    if (m_PictureNames.IndexOf(pic, StringComparison.OrdinalIgnoreCase) == -1)
                                    {
                                        if (File.Exists(PicOriginalFolder + pic)) File.Delete(PicOriginalFolder + pic);
                                        if (File.Exists(PicNormalFolder + pic)) File.Delete(PicNormalFolder + pic);
                                        if (File.Exists(PicThumbnailFolder + pic)) File.Delete(PicThumbnailFolder + pic);
                                        Log.Info("Picture deleted:" + pic);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Save pictures failed", ex);
            }
        }

        public string TempFolder2
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["temp"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("temp");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string DamFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["dam"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("DigitalAssets");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicNormalFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["picnormal"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picnormal");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicThumbnailFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["picthumbnail"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picthumbnail");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicOriginalFolder
        {
            get
            {
                string tmp = GetStr(System.Configuration.ConfigurationManager.AppSettings["picoriginal"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picoriginal");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        private bool fctResizeConvertImage(string strFile, string strDestination, int newWidth, int newHeight, bool blnDelete = false)
        {
            try
            {
                double dblTempW;
                double dblTempH;
                int H1;
                int W1;

                using var originalBitmap = (Bitmap)Bitmap.FromFile(strFile, true);
                decimal WidthVsHeightRatio = (decimal)originalBitmap.Height / (decimal)originalBitmap.Width;

                if (newHeight > newWidth)
                {
                    dblTempH = newHeight;
                    dblTempW = dblTempH / (double)WidthVsHeightRatio;
                }
                else
                {
                    dblTempW = newWidth;
                    dblTempH = dblTempW * (double)WidthVsHeightRatio;
                }
                while (dblTempW > newWidth || dblTempH > newHeight)
                {
                    dblTempW *= 0.999;
                    dblTempH *= 0.999;
                }
                W1 = (int)dblTempW;
                H1 = (int)dblTempH;

                using var newbmp = new Bitmap(W1, H1);
                using (var newg = Graphics.FromImage(newbmp))
                {
                    newg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    newg.Clear(Color.White);
                    newg.DrawImage(originalBitmap, 0, 0, W1, H1);
                }
                newbmp.Save(strDestination, System.Drawing.Imaging.ImageFormat.Jpeg);
                if (blnDelete && File.Exists(strFile)) File.Delete(strFile);
            }
            catch (Exception ex)
            {
                Log.Error("ResizeConvertImage failed", ex);
                return false;
            }
            return true;
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
