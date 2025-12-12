using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CookbookController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codeliste:int}")]
        public ActionResult<Models.CookbookData> GetCookbookById(int codeliste)
        {
            var data = new Models.CookbookData();
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_ProjectInfo]";
                cmd.Parameters.Add("@CodeProject", SqlDbType.Int).Value = codeliste;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

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
                            Picture = string.IsNullOrWhiteSpace(GetStr(dr["Picture"])) ? "default.png" : GetStr(dr["Picture"]),
                            hasPicture = GetBool(dr["hasPicture"]) 
                        };
                    }

                    if (tblSites.Rows.Count > 0)
                    {
                        data.Sharing = new List<Models.GenericList>();
                        foreach (DataRow dr in tblSites.Rows)
                        {
                            data.Sharing.Add(new Models.GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Name"]) });
                        }
                    }

                    if (tblUser.Rows.Count > 0)
                    {
                        data.Users = new List<Models.GenericList>();
                        foreach (DataRow dr in tblUser.Rows)
                        {
                            data.Users.Add(new Models.GenericList { Code = GetInt(dr["CodeUser"]), Value = GetStr(dr["IsAssigned"]), Name = GetStr(dr["Name"]), Value2 = GetInt(dr["CodeSite"]) });
                        }
                    }
                }
                return Ok(data);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{codeproperty:int?}/{name?}")]
        public ActionResult<List<Models.TreeNode>> GetCookbookByName(int codesite, int codetrans, int type, int tree, int codeliste, int codeproperty = -1, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Projects]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
                cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

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
                    foreach (var word in name.Trim().Split(','))
                    {
                        foreach (var k in projects)
                        {
                            if ((k.Name ?? string.Empty).ToLowerInvariant().Contains((word ?? string.Empty).Trim().ToLowerInvariant()))
                            {
                                if (!projectschildren.Contains(k)) projectschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in projectschildren)
                    {
                        projectsresult.Add(kid);
                        var currentParentCode = kid.ParentCode;
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
                    if (projectdata.All(obj => obj.key != p.Code))
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
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("cookbooklist/{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{name?}")]
        public ActionResult<Models.ResponseCookBook> GetCookbookAll(int codesite, int codetrans, int type, int tree, int codeliste, string? name = "", int skip = 0, int take = 10)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_ProjectList]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var projects = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    projects.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]) + " (" + GetStr(r["RecipeCount"]) + ")",
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var projectschildren = new List<Models.GenericTree>();
                    var projectsresult = new List<Models.GenericTree>();
                    foreach (var word in name.Trim().Split(','))
                    {
                        foreach (var k in projects)
                        {
                            if ((k.Name ?? string.Empty).ToLowerInvariant().Contains((word ?? string.Empty).Trim().ToLowerInvariant()))
                            {
                                if (!projectschildren.Contains(k)) projectschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in projectschildren)
                    {
                        projectsresult.Add(kid);
                        var currentParentCode = kid.ParentCode;
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
                    if (projectdata.All(obj => obj.key != p.Code))
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

                var totalCount = projectdata.Count;
                take = Math.Min(Math.Max(take, 1), totalCount);
                var totalPages = (int)Math.Ceiling(totalCount / (double)take);
                skip = Math.Min(Math.Max(skip, 0), totalPages);
                return Ok(new Models.ResponseCookBook(projectdata.Skip(take * skip).Take(take).ToList(), totalCount));
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("list/{codesite:int}/{codetrans:int}/{type:int}/{codeuser:int}/{codekey:int}/{name?}")]
        public ActionResult<Models.ResponseTreeGeneric> GetCookbookListAll(int codesite, int codetrans, int type, int codeuser, int codekey, string? name = "", int skip = 0, int take = 10)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_ProjectList]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@CodeKey", SqlDbType.Int).Value = codekey;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var projects = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    projects.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]) + " (" + GetStr(r["RecipeCount"]) + ")",
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var decoded = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(name));
                    var projectschildren = new List<Models.GenericTree>();
                    var projectsresult = new List<Models.GenericTree>();
                    foreach (var word in decoded.Trim().Split(','))
                    {
                        foreach (var k in projects)
                        {
                            if ((k.Name ?? string.Empty).ToLowerInvariant().Contains((word ?? string.Empty).Trim().ToLowerInvariant()))
                            {
                                if (!projectschildren.Contains(k)) projectschildren.Add(k);
                            }
                        }
                    }
                    foreach (var kid in projectschildren)
                    {
                        projectsresult.Add(kid);
                        var currentParentCode = kid.ParentCode;
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
                    if (projectdata.All(obj => obj.id != p.Code))
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

                var totalCount = projectdata.Count;
                take = Math.Min(Math.Max(take, 1), totalCount);
                var totalPages = (int)Math.Ceiling(totalCount / (double)take);
                skip = Math.Min(Math.Max(skip, 0), totalPages);
                return Ok(new Models.ResponseTreeGeneric(projectdata.Skip(take * skip).Take(take).ToList(), totalCount));
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("recipe/{codesite:int}/{codetrans:int}/{type:int}/{codeuser:int}/{codekey:int}/{name?}")]
        public ActionResult<Models.ResponseCookBookRecipe> GetCookbookRecipeByName(int codesite, int codetrans, int type, int codeuser, int codekey, string? name = "", int page = 0, int limit = 10)
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
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
                    using var da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
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
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_GET_ProjectRecipeList]";
                    cmd.Parameters.Add("@CodeKey", SqlDbType.Int).Value = codekey;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                    cn.Open();
                    using var da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
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
                var totalCount = recipeChild.Count;
                limit = Math.Min(Math.Max(limit, 1), totalCount);
                var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
                page = Math.Min(Math.Max(page, 0), totalPages);
                return Ok(new Models.ResponseCookBookRecipe(recipeChild.Skip(limit * (page - 1)).Take(limit).ToList(), totalCount));
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("{codesite:int}/{code:int}/{parentOnly:bool?}")]
        public ActionResult<List<Models.GenericList>> GetCookbookList(int codesite, int code, bool parentOnly = false)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
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
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var cookbooks = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cookbooks.Add(new Models.GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
                return Ok(cookbooks);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("sharing/{codeproject:int}")]
        public ActionResult<List<Models.TreeNode>> GetCookbookSharing(int codeproject)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_SharingProject]";
                cmd.Parameters.Add("@CodeProject", SqlDbType.Int).Value = codeproject;
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
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SaveCookbook([FromBody] Models.CookbookData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? trans = null;
            try
            {
                if (data?.Info == null) throw new ArgumentNullException("invalid cookbook data");
                var intCodeProject = GetInt(data.Info.Code);
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);

                var arrSharing = new ArrayList();
                foreach (var sh in data.Sharing) { if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code); }
                var codeSiteList = string.Join(",", arrSharing.Cast<object>());

                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SP_API_UPDATE_Project;
                cmd.Parameters.Clear();
                var pCode = cmd.Parameters.Add("@Code", SqlDbType.Int);
                pCode.Value = data.Info.Code;
                pCode.Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
                cmd.Parameters.Add("@Global", SqlDbType.Bit).Value = data.Info.Global;
                cmd.Parameters.Add("@CodeParent", SqlDbType.Int).Value = data.Info.ParentCode;
                cmd.Parameters.Add("@CodeOwner", SqlDbType.Int).Value = data.Info.CodeOwner;
                cmd.Parameters.Add("@CanBeParent", SqlDbType.Bit).Value = data.Info.CanBeParent;
                cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 400).Value = data.Info.Picture;
                cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 2000).Value = codeSiteList;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                ret.Direction = ParameterDirection.Output;

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();

                intCodeProject = GetInt(pCode.Value, -1);
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0) throw new Exception($"[{resultCode}] Save cookbook data failed");
                if (intCodeProject <= 0) throw new Exception($"[{resultCode}] Cookbook not created");

                if (data.Sharing != null)
                {
                    var sharingList = data.Sharing.Select(s => s.Code).Distinct().ToList();
                    var codeSharedTo = string.Join(",", sharingList);
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
                    if (resultCode != 0) throw new Exception($"[{resultCode}] Save cookbook sharing failed");
                }

                if (data.Users != null)
                {
                    var userList = data.Users.Where(u => u.Value == "1").Select(u => u.Code).Distinct().ToList();
                    var codeusers = string.Join(",", userList);
                    cmd.CommandText = SP_API_UPDATE_ProjectUser;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeProject", SqlDbType.Int).Value = intCodeProject;
                    cmd.Parameters.Add("@CodeUsers", SqlDbType.VarChar, 4000).Value = codeusers;
                    var retUsers = cmd.Parameters.Add("@Return", SqlDbType.Int);
                    retUsers.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(retUsers.Value, -1);
                    if (resultCode != 0) throw new Exception($"[{resultCode}] Save cookbook users failed");
                }

                trans.Commit();
                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = intCodeProject;
                response.Status = true;
            }
            catch (Exception)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save cookbook failed";
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteCookbook([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var arrProjectCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrProjectCodes.Contains(c.Code)) arrProjectCodes.Add(c.Code);
                }
                var codeProjectList = string.Join(",", arrProjectCodes.Cast<object>().Select(x => Convert.ToString(x)));

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
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
                if (resultCode != 0) throw new Exception($"[{resultCode}] Delete cookbook failed");

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = GetStr(skipList.Value);
                response.Status = true;
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.ReturnValue = GetStr("");
                response.Message = "Delete cookbook failed";
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        private static Models.GenericTree GetParent(int parentcode, List<Models.GenericTree> keywords)
        {
            var k = keywords.Where(obj => obj.Code == parentcode);
            return k.Any() ? k.First() : new Models.GenericTree();
        }

        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> data, int code)
        {
            var children = new List<Models.TreeNode>();
            var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new Models.TreeNode
                {
                    title = k.Name,
                    key = k.Code,
                    icon = false,
                    children = CreateChildren(data, k.Code),
                    select = k.Flagged,
                    selected = k.Flagged,
                    parenttitle = k.ParentName,
                    note = k.Note
                };
                children.Add(child);
            }
            return children;
        }

        private static List<Models.GenericTreeNode> CreateChildrenGeneric(List<Models.GenericTree> data, int code)
        {
            var children = new List<Models.GenericTreeNode>();
            var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new Models.GenericTreeNode
                {
                    title = k.Name,
                    id = k.Code,
                    children = CreateChildrenGeneric(data, k.Code)
                };
                children.Add(child);
            }
            return children;
        }

        private static List<Models.TreeGridNode> CreateChildrenRecipe(List<Models.GenericTree> data, int code)
        {
            var children = new List<Models.TreeGridNode>();
            var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new Models.TreeGridNode
                {
                    title = k.Name,
                    key = k.Code,
                    expanded = false
                };
                // TODO: load recipes under child if needed
                child.children = CreateChildrenRecipe(data, k.Code);
                children.Add(child);
            }
            return children;
        }

        private static string GetStr(object? value)
        {
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
        }
        private static int GetInt(object? value, int fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (int.TryParse(Convert.ToString(value), out var i)) return i;
            try { return Convert.ToInt32(value); } catch { return fallback; }
        }
        private static bool GetBool(object? value)
        {
            if (value == null || value == DBNull.Value) return false;
            if (bool.TryParse(Convert.ToString(value), out var b)) return b;
            try { return Convert.ToInt32(value) != 0; } catch { return false; }
        }

        public const string SP_API_UPDATE_Project = "API_UPDATE_Project";
        public const string SP_API_UPDATE_Sharing = "API_UPDATE_Sharing";
        public const string SP_API_UPDATE_ProjectUser = "API_UPDATE_ProjectUser";
    }

    // Placeholder models - replace with actual
    namespace Models
    {
        public class CookbookData { public Cookbook Info { get; set; } = new(); public List<GenericList> Sharing { get; set; } = new(); public List<GenericList> Users { get; set; } = new(); public Profile Profile { get; set; } = new(); }
        public class Cookbook { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public int CodeSite { get; set; } public int CodeOwner { get; set; } public bool Global { get; set; } public bool CanBeParent { get; set; } public string Picture { get; set; } = string.Empty; public bool hasPicture { get; set; } }
        public class GenericList { public int Code { get; set; } public string Value { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public int Value2 { get; set; } }
        public class Profile { public int Code { get; set; } public int CodeSite { get; set; } }
        public class GenericTree { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public string ParentName { get; set; } = string.Empty; public string Picture { get; set; } = string.Empty; public bool hasPicture { get; set; } public bool Flagged { get; set; } public string Note { get; set; } = string.Empty; }
        public class TreeNode { public string title { get; set; } = string.Empty; public int key { get; set; } public bool icon { get; set; } public List<TreeNode> children { get; set; } = new(); public bool select { get; set; } public bool selected { get; set; } public string parenttitle { get; set; } = string.Empty; public string note { get; set; } = string.Empty; }
        public class GenericTreeNode { public string title { get; set; } = string.Empty; public int id { get; set; } public List<GenericTreeNode> children { get; set; } = new(); }
        public class TreeGridNode { public string title { get; set; } = string.Empty; public int key { get; set; } public bool expanded { get; set; } public bool image { get; set; } public bool leaf { get; set; } public string number { get; set; } = string.Empty; public bool nutrition { get; set; } public string primarybrand { get; set; } = string.Empty; public string secondarybrand { get; set; } = string.Empty; public string status { get; set; } = string.Empty; public string subname { get; set; } = string.Empty; public string parenttitle { get; set; } = string.Empty; public int note { get; set; } public int CheckoutUser { get; set; } public string Picture { get; set; } = string.Empty; public bool IsLocked { get; set; } public List<TreeGridNode> children { get; set; } = new(); }
        public class ResponseCookBook { public ResponseCookBook(List<TreeNode> data, int total) { Data = data; Total = total; } public List<TreeNode> Data { get; set; } public int Total { get; set; } }
        public class ResponseTreeGeneric { public ResponseTreeGeneric(List<GenericTreeNode> data, int total) { Data = data; Total = total; } public List<GenericTreeNode> Data { get; set; } public int Total { get; set; } }
        public class ProjectRecipe { public int Code { get; set; } public string Name { get; set; } = string.Empty; public string Subname { get; set; } = string.Empty; public string PrimaryBrand { get; set; } = string.Empty; public string SecondaryBrand { get; set; } = string.Empty; public string Status { get; set; } = string.Empty; public bool Image { get; set; } public bool Nutrition { get; set; } public int CheckoutUser { get; set; } public string Number { get; set; } = string.Empty; public string Picture { get; set; } = string.Empty; public bool isLocked { get; set; } }
        public class ResponseCookBookRecipe { public ResponseCookBookRecipe(List<TreeGridNode> data, int total) { Data = data; Total = total; } public List<TreeGridNode> Data { get; set; } public int Total { get; set; } }
    }
}
