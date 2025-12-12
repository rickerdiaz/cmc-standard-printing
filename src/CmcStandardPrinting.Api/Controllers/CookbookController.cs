using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Linq;
using System.Text;
using CmcStandardPrinting.Domain.Cookbooks;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CookbookController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CookbookController> _logger;

    public CookbookController(IConfiguration configuration, ILogger<CookbookController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codeliste:int}")]
    public ActionResult<CookbookData> GetCookbookById(int codeliste)
    {
        var data = new CookbookData();
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

            if (ds.Tables.Count == 3)
            {
                var tblInfo = ds.Tables[0];
                var tblSites = ds.Tables[1];
                var tblUser = ds.Tables[2];

                foreach (DataRow dr in tblInfo.Rows)
                {
                    data.Info = new Cookbook
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        ParentCode = GetInt(dr["ParentCode"]),
                        CodeSite = GetInt(dr["CodeSite"]),
                        CodeOwner = GetInt(dr["CodeOwner"]),
                        Global = GetBool(dr["Global"]),
                        CanBeParent = GetBool(dr["CanBeParent"]),
                        Picture = string.IsNullOrWhiteSpace(GetStr(dr["Picture"])) ? "default.png" : GetStr(dr["Picture"]),
                        HasPicture = GetBool(dr["hasPicture"])
                    };
                }

                if (tblSites.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblSites.Rows)
                    {
                        data.Sharing.Add(new GenericList { Code = GetInt(dr["Code"]), Value = GetStr(dr["Name"]) });
                    }
                }

                if (tblUser.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblUser.Rows)
                    {
                        data.Users.Add(new GenericList { Code = GetInt(dr["CodeUser"]), Value = GetStr(dr["IsAssigned"]), Name = GetStr(dr["Name"]), Value2 = GetStr(dr["CodeSite"]) });
                    }
                }
            }

            return Ok(data);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCookbookById failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{codeproperty:int?}/{name?}")]
    public ActionResult<List<TreeNode>> GetCookbookByName(int codesite, int codetrans, int type, int tree, int codeliste, int codeproperty = -1, string? name = "")
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

            var projects = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                projects.Add(new GenericTree
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    ParentCode = GetInt(r["ParentCode"]),
                    ParentName = GetStr(r["ParentName"]),
                    Picture = GetStr(r["ParentName"]),
                    HasPicture = GetBool(r["hasPicture"]),
                    Flagged = GetBool(r["Flagged"])
                });
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var projectschildren = new List<GenericTree>();
                var projectsresult = new List<GenericTree>();
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

            var projectdata = new List<TreeNode>();
            var parents = projects.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (projectdata.All(obj => obj.Key != p.Code))
                {
                    var parent = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildren(projects, p.Code),
                        Select = p.Flagged,
                        Selected = p.Flagged,
                        ParentTitle = p.ParentName
                    };
                    projectdata.Add(parent);
                }
            }

            var totalCount = projectdata.Count;
            tree = Math.Min(Math.Max(tree, 1), totalCount);
            var totalPages = (int)Math.Ceiling(totalCount / (double)tree);
            codeliste = Math.Min(Math.Max(codeliste, 0), totalPages);
            return Ok(new ResponseCookBook(projectdata.Skip(tree * codeliste).Take(tree).ToList(), totalCount));
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCookbookByName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("cookbooklist/{codesite:int}/{codetrans:int}/{type:int}/{tree:int}/{codeliste:int}/{name?}")]
    public ActionResult<ResponseCookBook> GetCookbookAll(int codesite, int codetrans, int type, int tree, int codeliste, string? name = "", int skip = 0, int take = 10)
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

            var projects = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                projects.Add(new GenericTree
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
                var projectschildren = new List<GenericTree>();
                var projectsresult = new List<GenericTree>();
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

            var projectdata = new List<TreeNode>();
            var parents = projects.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (projectdata.All(obj => obj.Key != p.Code))
                {
                    var parent = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildren(projects, p.Code),
                        Select = p.Flagged,
                        Selected = p.Flagged,
                        ParentTitle = p.ParentName
                    };
                    projectdata.Add(parent);
                }
            }

            var totalCount = projectdata.Count;
            take = Math.Min(Math.Max(take, 1), totalCount);
            var totalPages = (int)Math.Ceiling(totalCount / (double)take);
            skip = Math.Min(Math.Max(skip, 0), totalPages);
            return Ok(new ResponseCookBook(projectdata.Skip(take * skip).Take(take).ToList(), totalCount));
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCookbookAll failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("list/{codesite:int}/{codetrans:int}/{type:int}/{codeuser:int}/{codekey:int}/{name?}")]
    public ActionResult<ResponseTreeGeneric> GetCookbookListAll(int codesite, int codetrans, int type, int codeuser, int codekey, string? name = "", int skip = 0, int take = 10)
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

            var projects = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                projects.Add(new GenericTree
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
                var decoded = Encoding.ASCII.GetString(Convert.FromBase64String(name));
                var projectschildren = new List<GenericTree>();
                var projectsresult = new List<GenericTree>();
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

            var projectdata = new List<GenericTreeNode>();
            var parents = projects.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                if (projectdata.All(obj => obj.Id != p.Code))
                {
                    var parent = new GenericTreeNode
                    {
                        Title = p.Name,
                        Id = p.Code,
                        Children = CreateChildrenGeneric(projects, p.Code)
                    };
                    projectdata.Add(parent);
                }
            }

            var totalCount = projectdata.Count;
            take = Math.Min(Math.Max(take, 1), totalCount);
            var totalPages = (int)Math.Ceiling(totalCount / (double)take);
            skip = Math.Min(Math.Max(skip, 0), totalPages);
            return Ok(new ResponseTreeGeneric(projectdata.Skip(take * skip).Take(take).ToList(), totalCount));
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCookbookListAll failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("recipe/{codesite:int}/{codetrans:int}/{type:int}/{codeuser:int}/{codekey:int}/{name?}")]
    public ActionResult<ResponseCookBookRecipe> GetCookbookRecipeByName(int codesite, int codetrans, int type, int codeuser, int codekey, string? name = "", int page = 0, int limit = 10)
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

            var projects = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                projects.Add(new GenericTree { Code = GetInt(r["Code"]), Name = GetStr(r["Name"]) });
            }

            var recipeChild = new List<TreeGridNode>();
            foreach (var p in projects)
            {
                var projectList = new TreeGridNode { Title = p.Name, Key = p.Code };
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

            var recipe = new List<ProjectRecipe>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                recipe.Add(new ProjectRecipe
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
                    IsLocked = GetBool(r["IsLocked"])
                });
            }

            foreach (var r in recipe)
            {
                var recipeList = new TreeGridNode
                {
                    Title = r.Name,
                    Key = r.Code,
                    Expanded = false,
                    Image = r.Image,
                    Leaf = true,
                    Number = r.Number,
                    Nutrition = r.Nutrition,
                    Primarybrand = r.PrimaryBrand,
                    Secondarybrand = r.SecondaryBrand,
                    Status = r.Status,
                    Subname = r.Subname,
                    CheckoutUser = r.CheckoutUser,
                    Picture = r.Picture,
                    IsLocked = r.IsLocked
                };
                recipeChild.Add(recipeList);
            }

            var totalCount = recipeChild.Count;
            limit = Math.Min(Math.Max(limit, 1), totalCount);
            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
            page = Math.Min(Math.Max(page, 0), totalPages);
            return Ok(new ResponseCookBookRecipe(recipeChild.Skip(limit * (page - 1)).Take(limit).ToList(), totalCount));
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCookbookRecipeByName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{code:int}/{parentOnly:bool?}")]
    public ActionResult<List<GenericList>> GetCookbookList(int codesite, int code, bool parentOnly = false)
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

            var cookbooks = new List<GenericList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                cookbooks.Add(new GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
            }

            return Ok(cookbooks);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCookbookList failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("sharing/{codeproject:int}")]
    public ActionResult<List<TreeNode>> GetCookbookSharing(int codeproject)
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

            var sharings = new List<GenericTree>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                sharings.Add(new GenericTree
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

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new TreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = CreateChildrenSharing(sharings, p.Code),
                    Select = p.Flagged,
                    Selected = p.Flagged,
                    ParentTitle = p.ParentName,
                    GroupLevel = GroupLevel.Property
                };

                if (parent.Children.Count > 0) sharingdata.Add(parent);
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCookbookSharing failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveCookbook([FromBody] CookbookData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            if (data?.Info == null) throw new ArgumentNullException("invalid cookbook data");
            var intCodeProject = GetInt(data.Info.Code);
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);

            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing)
            {
                if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
            }

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
        catch (Exception ex)
        {
            try
            {
                trans?.Rollback();
                trans?.Dispose();
            }
            catch
            {
            }

            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Save cookbook failed";
            _logger.LogError(ex, "SaveCookbook failed");
            return StatusCode(500, response);
        }

        return Ok(response);
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeleteCookbook([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
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
        catch (Exception ex)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = GetStr(string.Empty);
            response.Message = "Delete cookbook failed";
            _logger.LogError(ex, "DeleteCookbook failed");
            return StatusCode(500, response);
        }

        return Ok(response);
    }

    private static GenericTree GetParent(int parentcode, List<GenericTree> keywords)
    {
        var k = keywords.Where(obj => obj.Code == parentcode);
        return k.Any() ? k.First() : new GenericTree();
    }

    private static List<TreeNode> CreateChildren(List<GenericTree> data, int code)
    {
        var children = new List<TreeNode>();
        var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = CreateChildren(data, k.Code),
                Select = k.Flagged,
                Selected = k.Flagged,
                ParentTitle = k.ParentName,
                Note = k.Note
            };
            children.Add(child);
        }

        return children;
    }

    private static List<GenericTreeNode> CreateChildrenGeneric(List<GenericTree> data, int code)
    {
        var children = new List<GenericTreeNode>();
        var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new GenericTreeNode
            {
                Title = k.Name,
                Id = k.Code,
                Children = CreateChildrenGeneric(data, k.Code)
            };
            children.Add(child);
        }

        return children;
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> data, int code)
    {
        var children = new List<TreeNode>();
        var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = CreateChildrenSharing(data, k.Code),
                Select = k.Flagged,
                Selected = k.Flagged,
                ParentTitle = k.ParentName,
                GroupLevel = (GroupLevel)k.Type,
                Note = k.Note
            };
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
