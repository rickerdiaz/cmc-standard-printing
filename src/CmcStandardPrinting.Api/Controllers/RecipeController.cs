using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Globalization;
using System.Linq;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Recipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(IConfiguration configuration, ILogger<RecipeController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("printingstatus")]
    public ActionResult<DataTable> GetPrintingStatus()
    {
        var dt = new DataTable();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM EgswLabelToPrint";
            cmd.CommandType = CommandType.Text;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for printing status");
        }
        catch (HttpResponseException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading printing status");
        }

        return Ok(dt);
    }

    [HttpGet("status/{type:int}/{codetrans:int}")]
    public ActionResult<List<GenericList>> GetRecipeStatus(int type, int codetrans)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_EGSWSTATUSList]";
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<GenericList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch recipe status");
            return StatusCode(500);
        }
    }

    [HttpGet("usedasingredient/{codetrans:int}/{codeliste:int}")]
    public ActionResult<List<RecipeUsedAsIngredient>> GetUsedAsIngredient(int codetrans, int codeliste)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_RecipeUsedAsIngredient]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<RecipeUsedAsIngredient>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new RecipeUsedAsIngredient
                {
                    Code = GetInt(r["Code"]),
                    Number = GetStr(r["Number"]),
                    Name = GetStr(r["Name"])
                });
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch recipes used as ingredient");
            return StatusCode(500);
        }
    }

    [HttpPost("weight/{codeliste:int}/{codesetprice}")]
    public ActionResult<ResponseCallBack> GetRecipeWeight(int codeliste, int codesetprice, [FromBody] IngredientWeightList ingredients)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"DECLARE @Weight AS FLOAT; EXEC @Weight = dbo.[fn_EgsWGETRecipeWeightActual] @p_nCodeListe=@p_nCodeListe, @p_nCodeSetPrice = @p_nCodeSetPrice SELECT @Weight AS [Weight]";
            cn.Open();
            cmd.Parameters.Add("@p_nCodeListe", SqlDbType.Int).Value = GetInt(codeliste);
            cmd.Parameters.Add("@p_nCodeSetPrice", SqlDbType.Int).Value = GetInt(codesetprice);
            double totalWeight = GetDbl(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT Factor FROM dbo.EgswUnit WHERE Code = @DisplayCodeUnit";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@DisplayCodeUnit", SqlDbType.Int).Value = GetInt(ingredients.DisplayCodeUnit);
            var unitFactor = GetDbl(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT Format FROM dbo.EgswUnit WHERE Code = @DisplayCodeUnit";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@DisplayCodeUnit", SqlDbType.Int).Value = GetInt(ingredients.DisplayCodeUnit);
            var unitFormat = GetStr(cmd.ExecuteScalar());

            totalWeight = totalWeight > 0 && unitFactor > 0 ? totalWeight / unitFactor : 0;

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = totalWeight.ToString(unitFormat, CultureInfo.InvariantCulture);
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            response.Status = false;
            return BadRequest(response);
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to compute recipe weight");
            response.Code = 500;
            response.Message = "Unexpected error occured";
            response.Status = false;
            return StatusCode(500, response);
        }
    }

    [HttpGet("composition/actual/{codeliste:int}/{codetrans}/{ishtml}/{codesetprice}/{isdisplayingredient}/{isdisplayweightperc}")]
    public ActionResult<ResponseCallBack> GetRecipeCompositionActual(int codeliste, int codetrans, bool ishtml, int codesetprice, bool isdisplayingredient, bool isdisplayweightperc)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"DECLARE @strComposition AS NVARCHAR(4000); SELECT @strComposition = dbo.[GetEULaw](@CodeListe,@IsHtml,@CodeTrans,@CodeSetPrice,@IsDisplayIngredient,@IsDisplayWeightPerc) SELECT @strComposition AS Composition";
            cn.Open();
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(codeliste);
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = GetInt(codetrans);
            cmd.Parameters.Add("@IsHtml", SqlDbType.Bit).Value = GetBool(ishtml);
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = GetInt(codesetprice);
            cmd.Parameters.Add("@IsDisplayIngredient", SqlDbType.Bit).Value = GetBool(isdisplayingredient);
            cmd.Parameters.Add("@IsDisplayWeightPerc", SqlDbType.Bit).Value = GetBool(isdisplayweightperc);
            var composition = GetStr(cmd.ExecuteScalar());
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = composition;
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            return BadRequest(response);
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get composition actual");
            response.Code = 500;
            response.Message = "Unexpected error occured";
            return StatusCode(500, response);
        }
    }

    [HttpGet("composition/swisslaw/{codeliste:int}/{codetrans}/{codesetprice}/{issubrecipe}")]
    public ActionResult<ResponseCallBack> GetRecipeSwissLaw(int codeliste, int codetrans, int codesetprice, bool issubrecipe)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"DECLARE @strComposition AS NVARCHAR(4000); SELECT @strComposition = dbo.[GetSwissLaw](@CodeListe,@CodeTrans,@CodeSetPrice,@IsSubrecipe) SELECT @strComposition AS Composition";
            cn.Open();
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(codeliste);
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = GetInt(codetrans);
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = GetInt(codesetprice);
            cmd.Parameters.Add("@IsSubrecipe", SqlDbType.Bit).Value = GetBool(issubrecipe);
            var composition = GetStr(cmd.ExecuteScalar());
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = composition;
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            return BadRequest(response);
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get swiss law composition");
            response.Code = 500;
            response.Message = "Unexpected error occured";
            return StatusCode(500, response);
        }
    }

    [HttpGet("composition/genericlaw/{codeliste:int}/{codetrans}/{codesetprice}/{issubrecipe}/{comptype}")]
    public ActionResult<ResponseCallBack> GetRecipeGenericLaw(int codeliste, int codetrans, int codesetprice, bool issubrecipe, int comptype)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"DECLARE @strComposition AS NVARCHAR(4000); SELECT @strComposition = dbo.[GetGenericLaw](@CodeListe,@CodeTrans,@CodeSetPrice,@IsSubrecipe,@CompType) SELECT @strComposition AS Composition";
            cn.Open();
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(codeliste);
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = GetInt(codetrans);
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = GetInt(codesetprice);
            cmd.Parameters.Add("@IsSubrecipe", SqlDbType.Bit).Value = GetBool(issubrecipe);
            cmd.Parameters.Add("@CompType", SqlDbType.Int).Value = GetInt(comptype);
            var composition = GetStr(cmd.ExecuteScalar());
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = composition;
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            return BadRequest(response);
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get generic composition");
            response.Code = 500;
            response.Message = "Unexpected error occured";
            return StatusCode(500, response);
        }
    }

    [HttpGet("composition/getallsubingredients/{codeliste:int}/{codetrans}/{codesetprice}/{isdisplaypercentage}")]
    public ActionResult<ResponseCallBack> GetAllSubIngredients(int codeliste, int codetrans, int codesetprice, bool isdisplaypercentage)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"DECLARE @strComposition AS NVARCHAR(4000); SELECT @strComposition = dbo.[GetAllSubIngredients](@CodeListe,@CodeTrans,@CodeSetPrice,@IsDisplayPercentage) SELECT @strComposition AS Composition";
            cn.Open();
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(codeliste);
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = GetInt(codetrans);
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = GetInt(codesetprice);
            cmd.Parameters.Add("@IsDisplayPercentage", SqlDbType.Bit).Value = GetBool(isdisplaypercentage);
            var composition = GetStr(cmd.ExecuteScalar());
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = composition;
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            return BadRequest(response);
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sub ingredient composition");
            response.Code = 500;
            response.Message = "Unexpected error occured";
            return StatusCode(500, response);
        }
    }

    [HttpGet("getTranslationLanguage/{codetrans:int}")]
    public ActionResult<ResponseCallBack> GetTranslationLanguage(int codetrans)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"SELECT L.Language FROM EgswTranslation T INNER JOIN EgswLanguage L ON L.Code = T.CodeDictionary WHERE T.Code = @CodeTrans";
            cn.Open();
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = GetInt(codetrans);
            var lang = GetStr(cmd.ExecuteScalar());
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = lang;
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            return Ok(response);
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get translation language");
            return Ok(response);
        }
    }

    [HttpPost("checkout")]
    public ActionResult<ResponseCallBack> CheckOut([FromBody] RecipeCheckout data)
    {
        var response = new ResponseCallBack();
        try
        {
            if (data == null)
            {
                return BadRequest(new ResponseCallBack { Code = 400, Message = "Missing or invalid parameters" });
            }

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE EgswListe SET CheckOutUser=@CodeUser, sLastAccess=GETDATE() WHERE Code=@CodeListe";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(data.CodeListe, -1);
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = GetInt(data.CodeUser, 0);
            cn.Open();
            cmd.ExecuteNonQuery();
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = data.CodeListe;
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            return BadRequest(new ResponseCallBack { Code = 400, Message = "Missing or invalid parameters" });
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Checkout failed");
            return StatusCode(500, new ResponseCallBack { Code = 500, Message = "Unexpected error occured" });
        }
    }

    [HttpGet("/api/recipe/ischeckout/{codeliste:int}")]
    public ActionResult<GenericList> IsCheckout(int codeliste)
    {
        var result = new GenericList();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT @CodeUser = ISNULL(CheckOutUser,0) FROM [dbo].[EgswListe] WITH(NOLOCK) WHERE Code = @CodeListe";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(codeliste, -1);
            var outParam = cmd.Parameters.Add("@CodeUser", SqlDbType.Int);
            outParam.Direction = ParameterDirection.Output;
            cn.Open();
            cmd.ExecuteNonQuery();
            result.Code = GetInt(outParam.Value, -1);
            result.Value = string.Empty;
            return Ok(result);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check checkout state");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/recipelink/{codetrans:int}/{codeliste:int}/{link:int}")]
    public ActionResult<object> GetRelatedRecipes(int codetrans, int codeliste, int link)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_RecipeRelated]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@link", SqlDbType.Int).Value = link;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            if (link > 0)
            {
                var recipes = new List<GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    recipes.Add(new GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        Link = GetStr(r["Link"]),
                        Flagged = r.Table.Columns.Contains("Flag") && GetBool(r["Flag"]),
                        Note = GetStr(r["Note"])
                    });
                }

                var list = new List<TreeNode>();
                var parents = recipes.Where(o => o.ParentCode == 0).OrderBy(o => o.Name).ToList();
                foreach (var p in parents)
                {
                    if (list.All(o => o.Key != p.Code))
                    {
                        var parent = new TreeNode
                        {
                            Title = p.Name,
                            Key = p.Code,
                            Icon = false,
                            Children = CreateChildren(recipes, p.Code),
                            Select = p.Flagged,
                            Selected = p.Flagged,
                            ParentTitle = p.ParentName,
                            Note = p.Note,
                            Link = p.Link
                        };
                        list.Add(parent);
                    }
                }

                return Ok(list);
            }

            var result = new List<GenericCodeValueList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                result.Add(new GenericCodeValueList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
            }

            return Ok(result);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get related recipes");
            return StatusCode(500);
        }
    }

    [HttpPost("/api/recipelink/search")]
    public ActionResult<object> GetRelatedRecipes([FromBody] ConfigurationcSearch data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_RecipeRelated]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = data.CodeListe;
            cmd.Parameters.Add("@link", SqlDbType.Int).Value = data.Link;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (data.Link > 0)
                {
                    var recipes = new List<GenericTree>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        recipes.Add(new GenericTree
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["Name"]),
                            ParentCode = GetInt(r["ParentCode"]),
                            Link = GetStr(r["Link"]),
                            Flagged = r.Table.Columns.Contains("Flag") && GetBool(r["Flag"]),
                            Note = GetStr(r["Note"])
                        });
                    }

                    var list = new List<TreeNode>();
                    var parents = recipes.Where(o => o.ParentCode == 0).OrderBy(o => o.Name).ToList();
                    foreach (var p in parents)
                    {
                        if (list.All(o => o.Key != p.Code))
                        {
                            var parent = new TreeNode
                            {
                                Title = p.Name,
                                Key = p.Code,
                                Icon = false,
                                Children = CreateChildren(recipes, p.Code),
                                Select = p.Flagged,
                                Selected = p.Flagged,
                                ParentTitle = p.ParentName,
                                Note = p.Note,
                                Link = p.Link
                            };
                            list.Add(parent);
                        }
                    }

                    return Ok(list);
                }
                else
                {
                    var result = new List<GenericList>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        result.Add(new GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                    }

                    return Ok(result);
                }
            }

            return Ok(Array.Empty<object>());
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search related recipes");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/merchandiselink/{codetrans:int}/{codeliste:int}/{link:int}")]
    public ActionResult<object> GetListRecipes(int codetrans, int codeliste, int link)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_RecipeRelated]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@link", SqlDbType.Int).Value = link;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (link > 0)
                {
                    var recipes = new List<GenericTree>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        recipes.Add(new GenericTree
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["Name"]),
                            ParentCode = GetInt(r["ParentCode"]),
                            Link = GetStr(r["Link"]),
                            Note = GetStr(r["Note"])
                        });
                    }

                    var list = new List<TreeNode>();
                    var parents = recipes.Where(o => o.ParentCode == 0).OrderBy(o => o.Name).ToList();
                    foreach (var p in parents)
                    {
                        if (list.All(o => o.Key != p.Code))
                        {
                            var parent = new TreeNode
                            {
                                Title = p.Name,
                                Key = p.Code,
                                Icon = false,
                                Children = CreateChildren(recipes, p.Code),
                                Select = p.Flagged,
                                ParentTitle = p.ParentName,
                                Note = p.Note,
                                Link = p.Link
                            };
                            list.Add(parent);
                        }
                    }

                    return Ok(list);
                }
                else
                {
                    var result = new List<GenericList>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        result.Add(new GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                    }

                    return Ok(result);
                }
            }

            return Ok(Array.Empty<object>());
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get merchandise link data");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/listefiles/{firstcode?}/{secondcode?}/{type?}")]
    public ActionResult<object> GetListeFiles(int firstcode = -1, int secondcode = -1, int type = -1)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_ListeFiles]";
            cmd.Parameters.Add("@FirstCode", SqlDbType.Int).Value = firstcode;
            cmd.Parameters.Add("@SecondCode", SqlDbType.Int).Value = secondcode;
            cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<ListeFiles>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new ListeFiles
                {
                    Code = GetInt(r["Code"]),
                    Pictures = GetStr(r["Pictures"]),
                    Videos = GetStr(r["Videos"])
                });
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get liste files");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/ispriceused/{intID?}")]
    public ActionResult<bool> IsPriceUsed(int intID)
    {
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_egswIsPriceUsed]";
            cmd.Parameters.Add("@intID", SqlDbType.Int).Value = intID;
            var outParam = cmd.Parameters.Add("@IsUsed", SqlDbType.Int);
            outParam.Direction = ParameterDirection.Output;
            cn.Open();
            cmd.ExecuteNonQuery();
            return Ok(Convert.ToInt32(outParam.Value) != 0);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check price usage");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/recipelabel/{codeliste?}")]
    public ActionResult<Label> GetRecipeLabel(int codeliste)
    {
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Labels]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cn.Open();
            var label = new Label();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    label.Code = GetInt(dr["Code"]);
                    label.CodeListe = GetInt(dr["CodeListe"]);
                    label.DeclarationName = GetStr(dr["DeclarationName"]);
                    label.SpecificDetermination = GetStr(dr["SpecificDetermination"]);
                    label.Number = GetStr(dr["Number"]);
                    label.Barcode = GetStr(dr["Barcode"]);
                    label.Consumption = GetDbl(dr["Consumption"]);
                    label.Sold = GetDbl(dr["Sold"]);
                    label.Composition = GetStr(dr["Composition"]);
                    label.Weight = GetDbl(dr["Weight"]);
                    label.Price = GetDbl(dr["Price"]);
                    label.PriceFor = GetDbl(dr["PriceFor"]);
                    label.Calculated = GetDbl(dr["Calculated"]);
                    label.Unit = GetInt(dr["Unit"]);
                    label.Note1 = GetInt(dr["Note1"]);
                    label.Note2 = GetInt(dr["Note2"]);
                    label.Note3 = GetInt(dr["Note3"]);
                    label.Certification = GetInt(dr["Certification"]);
                    label.CountryOfProduction = GetInt(dr["CountryOfProduction"]);
                    label.PackagingMethod = GetInt(dr["PackagingMethod"]);
                    label.Storage = GetInt(dr["Storage"]);
                }
            }

            return Ok(label);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load recipe label");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/recipelabelcomposition/{codeliste?}")]
    public ActionResult<List<LabelComposition>> GetRecipeLabelComposition(int codeliste)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_LabelsComposition]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<LabelComposition>();
            foreach (DataRow lc in ds.Tables[0].Rows)
            {
                list.Add(new LabelComposition
                {
                    CodeListe = GetInt(lc["CodeListe"]),
                    CodeTrans = GetInt(lc["CodeTrans"]),
                    CompoType = GetInt(lc["CompoType"]),
                    ImposedComposition = GetStr(lc["ImposedComposition"]),
                    IsValidated = GetBool(lc["IsValidated"])
                });
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load label composition");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/recipe/getTranslationLanguage/{codetrans:int}")]
    public ActionResult<string> GetTranslationLanguageRaw(int codetrans)
    {
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"SELECT L.Language FROM EgswTranslation T INNER JOIN EgswLanguage L ON L.Code = T.CodeDictionary WHERE T.Code = @CodeTrans";
            cn.Open();
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            return Ok(GetStr(cmd.ExecuteScalar()));
        }
        catch
        {
            return Ok(string.Empty);
        }
    }

    [HttpGet("/api/recipe/allergen/trace/{codeliste}/{codetrans?}/{foodlaw?}")]
    public ActionResult<ResponseCallBack> GetAllergenTraces(int codeliste, int codetrans, int foodlaw = 0)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cn.Open();
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT dbo.[fn_GetAllergenText] (@CodeListe, @CodeTrans, 2, 2)";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            var result = GetStr(cmd.ExecuteScalar());
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = result;
            response.Status = true;
            return Ok(response);
        }
        catch (ArgumentException)
        {
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            return BadRequest(response);
        }
        catch (HttpResponseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get allergen traces");
            response.Code = 500;
            response.Message = "Unexpected error occured";
            return StatusCode(500, response);
        }
    }

    private static List<TreeNode> CreateChildren(List<GenericTree> recipe, int code)
    {
        var children = new List<TreeNode>();
        if (recipe == null)
        {
            return children;
        }

        var kids = recipe.Where(o => o.Code != code && o.ParentCode == code && code > 0).OrderBy(o => o.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = CreateChildren(recipe, k.Code),
                Select = k.Flagged,
                ParentTitle = k.ParentName,
                Note = k.Note,
                Link = k.Link
            };
            children.Add(child);
        }

        return children;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i;
        try
        {
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return fallback;
        }
    }

    private static double GetDbl(object? value, double fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
        try
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return fallback;
        }
    }

    private static string GetStr(object? value, string fallback = "")
    {
        if (value == null || value == DBNull.Value) return fallback;
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }
}
