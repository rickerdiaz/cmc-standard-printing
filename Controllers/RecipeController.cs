using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;
        private IConfiguration Config => HttpContext.RequestServices.GetService<IConfiguration>();

        [HttpGet("api/recipe/printingstatus")]
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
            catch (ArgumentException) { }
            catch (HttpResponseException) { throw; }
            catch (Exception) { }
            return Ok(dt);
        }

        [HttpGet("/api/recipe/status/{type:int}/{codetrans:int}")]
        public ActionResult<List<Models.GenericList>> GetRecipeStatus(int type, int codetrans)
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
                var list = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                }
                return Ok(list);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/recipe/usedasingredient/{codetrans:int}/{codeliste:int}")]
        public ActionResult<List<Models.RecipeUsedAsIngredient>> GetUsedAsIngredient(int codetrans, int codeliste)
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
                var list = new List<Models.RecipeUsedAsIngredient>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.RecipeUsedAsIngredient
                    {
                        Code = GetInt(r["Code"]),
                        Number = GetStr(r["Number"]),
                        Name = GetStr(r["Name"])
                    });
                }
                return Ok(list);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/recipe/weight/{codeliste:int}/{codesetprice}")]
        public ActionResult<Models.ResponseCallBack> GetRecipeWeight(int codeliste, int codesetprice, [FromBody] Models.IngredientWeightList ingredients)
        {
            var response = new Models.ResponseCallBack();
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

                totalWeight = (totalWeight > 0 && unitFactor > 0) ? totalWeight / unitFactor : 0;

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = totalWeight.ToString(unitFormat, CultureInfo.InvariantCulture);
                response.Status = true;
                return Ok(response);
            }
            catch (ArgumentException)
            {
                response.Code = 400; response.Message = "Missing or invalid parameters"; response.Status = false; return BadRequest(response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Code = 500; response.Message = "Unexpected error occured"; response.Status = false; return StatusCode(500, response);
            }
        }

        [HttpGet("api/recipe/composition/actual/{codeliste:int}/{codetrans}/{ishtml}/{codesetprice}/{isdisplayingredient}/{isdisplayweightperc}")]
        public ActionResult<Models.ResponseCallBack> GetRecipeCompositionActual(int codeliste, int codetrans, bool ishtml, int codesetprice, bool isdisplayingredient, bool isdisplayweightperc)
        {
            var response = new Models.ResponseCallBack();
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
                response.Code = 0; response.Message = "OK"; response.ReturnValue = composition; response.Status = true; return Ok(response);
            }
            catch (ArgumentException) { response.Code = 400; response.Message = "Missing or invalid parameters"; return BadRequest(response); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { response.Code = 500; response.Message = "Unexpected error occured"; return StatusCode(500, response); }
        }

        [HttpGet("api/recipe/composition/swisslaw/{codeliste:int}/{codetrans}/{codesetprice}/{issubrecipe}")]
        public ActionResult<Models.ResponseCallBack> GetRecipeSwissLaw(int codeliste, int codetrans, int codesetprice, bool issubrecipe)
        {
            var response = new Models.ResponseCallBack();
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
                response.Code = 0; response.Message = "OK"; response.ReturnValue = composition; response.Status = true; return Ok(response);
            }
            catch (ArgumentException) { response.Code = 400; response.Message = "Missing or invalid parameters"; return BadRequest(response); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { response.Code = 500; response.Message = "Unexpected error occured"; return StatusCode(500, response); }
        }

        [HttpGet("api/recipe/composition/genericlaw/{codeliste:int}/{codetrans}/{codesetprice}/{issubrecipe}/{comptype}")]
        public ActionResult<Models.ResponseCallBack> GetRecipeGenericLaw(int codeliste, int codetrans, int codesetprice, bool issubrecipe, int comptype)
        {
            var response = new Models.ResponseCallBack();
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
                response.Code = 0; response.Message = "OK"; response.ReturnValue = composition; response.Status = true; return Ok(response);
            }
            catch (ArgumentException) { response.Code = 400; response.Message = "Missing or invalid parameters"; return BadRequest(response); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { response.Code = 500; response.Message = "Unexpected error occured"; return StatusCode(500, response); }
        }

        [HttpGet("api/recipe/composition/getallsubingredients/{codeliste:int}/{codetrans}/{codesetprice}/{isdisplaypercentage}")]
        public ActionResult<Models.ResponseCallBack> GetAllSubIngredients(int codeliste, int codetrans, int codesetprice, bool isdisplaypercentage)
        {
            var response = new Models.ResponseCallBack();
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
                response.Code = 0; response.Message = "OK"; response.ReturnValue = composition; response.Status = true; return Ok(response);
            }
            catch (ArgumentException) { response.Code = 400; response.Message = "Missing or invalid parameters"; return BadRequest(response); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { response.Code = 500; response.Message = "Unexpected error occured"; return StatusCode(500, response); }
        }

        [HttpGet("api/recipe/getTranslationLanguage/{codetrans:int}")]
        public ActionResult<Models.ResponseCallBack> GetTranslationLanguage(int codetrans)
        {
            var response = new Models.ResponseCallBack();
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
                response.Code = 0; response.Message = "OK"; response.ReturnValue = lang; response.Status = true; return Ok(response);
            }
            catch (ArgumentException) { return Ok(response); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return Ok(response); }
        }

        [HttpPost("api/recipe/checkout")]
        public ActionResult<Models.ResponseCallBack> CheckOut([FromBody] Models.RecipeCheckout data)
        {
            var response = new Models.ResponseCallBack();
            try
            {
                if (data == null) return BadRequest(new Models.ResponseCallBack { Code = 400, Message = "Missing or invalid parameters" });
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE EgswListe SET CheckOutUser=@CodeUser, sLastAccess=GETDATE() WHERE Code=@CodeListe";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(data.CodeListe, -1);
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = GetInt(data.CodeUser, 0);
                cn.Open();
                cmd.ExecuteNonQuery();
                response.Code = 0; response.Message = "OK"; response.ReturnValue = data.CodeListe; response.Status = true; return Ok(response);
            }
            catch (ArgumentException) { return BadRequest(new Models.ResponseCallBack { Code = 400, Message = "Missing or invalid parameters" }); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500, new Models.ResponseCallBack { Code = 500, Message = "Unexpected error occured" }); }
        }

        [HttpGet("/api/recipe/ischeckout/{codeliste:int}")]
        public ActionResult<Models.GenericList> IsCheckout(int codeliste)
        {
            var result = new Models.GenericList();
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
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
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
                    var recipes = new List<Models.GenericTree>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        recipes.Add(new Models.GenericTree
                        {
                            Code = GetInt(r["Code"]),
                            Name = GetStr(r["Name"]),
                            ParentCode = GetInt(r["ParentCode"]),
                            link = GetStr(r["Link"]),
                            Flagged = GetBool(r.Table.Columns.Contains("Flag") ? r["Flag"] : null),
                            Note = GetStr(r["Note"]) 
                        });
                    }
                    var list = new List<Models.TreeNode>();
                    var parents = recipes.Where(o => o.ParentCode == 0).OrderBy(o => o.Name).ToList();
                    foreach (var p in parents)
                    {
                        if (list.All(o => o.key != p.Code))
                        {
                            var parent = new Models.TreeNode
                            {
                                title = p.Name,
                                key = p.Code,
                                icon = false,
                                children = CreateChildren(recipes, p.Code),
                                select = p.Flagged,
                                selected = p.Flagged,
                                parenttitle = p.ParentName,
                                note = p.Note,
                                link = p.link
                            };
                            list.Add(parent);
                        }
                    }
                    return Ok(list);
                }
                else
                {
                    var result = new List<Models.GenericCodeValueList>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        result.Add(new Models.GenericCodeValueList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                    }
                    return Ok(result);
                }
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("/api/recipelink/search")]
        public ActionResult<object> GetRelatedRecipes([FromBody] Models.ConfigurationcSearch data)
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
                        var recipes = new List<Models.GenericTree>();
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            recipes.Add(new Models.GenericTree
                            {
                                Code = GetInt(r["Code"]),
                                Name = GetStr(r["Name"]),
                                ParentCode = GetInt(r["ParentCode"]),
                                link = GetStr(r["Link"]),
                                Flagged = GetBool(r.Table.Columns.Contains("Flag") ? r["Flag"] : null),
                                Note = GetStr(r["Note"]) 
                            });
                        }
                        var list = new List<Models.TreeNode>();
                        var parents = recipes.Where(o => o.ParentCode == 0).OrderBy(o => o.Name).ToList();
                        foreach (var p in parents)
                        {
                            if (list.All(o => o.key != p.Code))
                            {
                                var parent = new Models.TreeNode
                                {
                                    title = p.Name,
                                    key = p.Code,
                                    icon = false,
                                    children = CreateChildren(recipes, p.Code),
                                    select = p.Flagged,
                                    selected = p.Flagged,
                                    parenttitle = p.ParentName,
                                    note = p.Note,
                                    link = p.link
                                };
                                list.Add(parent);
                            }
                        }
                        return Ok(list);
                    }
                    else
                    {
                        var result = new List<Models.GenericList>();
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            result.Add(new Models.GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                        }
                        return Ok(result);
                    }
                }
                return Ok(Array.Empty<object>());
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
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
                        var recipes = new List<Models.GenericTree>();
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            recipes.Add(new Models.GenericTree
                            {
                                Code = GetInt(r["Code"]),
                                Name = GetStr(r["Name"]),
                                ParentCode = GetInt(r["ParentCode"]),
                                link = GetStr(r["Link"]),
                                Note = GetStr(r["Note"]) 
                            });
                        }
                        var list = new List<Models.TreeNode>();
                        var parents = recipes.Where(o => o.ParentCode == 0).OrderBy(o => o.Name).ToList();
                        foreach (var p in parents)
                        {
                            if (list.All(o => o.key != p.Code))
                            {
                                var parent = new Models.TreeNode
                                {
                                    title = p.Name,
                                    key = p.Code,
                                    icon = false,
                                    children = CreateChildren(recipes, p.Code),
                                    select = p.Flagged,
                                    parenttitle = p.ParentName,
                                    note = p.Note,
                                    link = p.link
                                };
                                list.Add(parent);
                            }
                        }
                        return Ok(list);
                    }
                    else
                    {
                        var result = new List<Models.GenericList>();
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            result.Add(new Models.GenericList { Code = GetInt(r["Code"]), Value = GetStr(r["Name"]) });
                        }
                        return Ok(result);
                    }
                }
                return Ok(Array.Empty<object>());
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
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
                var list = new List<Models.ListeFiles>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.ListeFiles
                    {
                        Code = GetInt(r["Code"]),
                        Pictures = GetStr(r["Pictures"]),
                        Videos = GetStr(r["Videos"])
                    });
                }
                return Ok(list);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
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
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/recipelabel/{codeliste?}")]
        public ActionResult<Models.Label> GetRecipeLabel(int codeliste)
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
                var label = new Models.Label();
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
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/recipelabelcomposition/{codeliste?}")]
        public ActionResult<List<Models.LabelComposition>> GetRecipeLabelComposition(int codeliste)
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
                var list = new List<Models.LabelComposition>();
                foreach (DataRow lc in ds.Tables[0].Rows)
                {
                    list.Add(new Models.LabelComposition
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
            catch (ArgumentException) { return BadRequest(); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
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
            catch { return Ok(string.Empty); }
        }

        [HttpGet("/api/recipe/allergen/trace/{codeliste}/{codetrans?}/{foodlaw?}")]
        public ActionResult<Models.ResponseCallBack> GetAllergenTraces(int codeliste, int codetrans, int foodlaw = 0)
        {
            var response = new Models.ResponseCallBack();
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
                response.Code = 0; response.Message = "OK"; response.ReturnValue = result; response.Status = true; return Ok(response);
            }
            catch (ArgumentException) { response.Code = 400; response.Message = "Missing or invalid parameters"; return BadRequest(response); }
            catch (HttpResponseException) { throw; }
            catch (Exception) { response.Code = 500; response.Message = "Unexpected error occured"; return StatusCode(500, response); }
        }

        // Helpers translated from VB
        private static int GetInt(object value, int fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (value is int i) return i;
            if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
            try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
        }
        private static double GetDbl(object value, double fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (value is double d) return d;
            if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var dd)) return dd;
            try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); } catch { return fallback; }
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
        private static DateTime GetDate(object value, DateTime? fallback = null)
        {
            if (value == null || value == DBNull.Value) return fallback ?? DateTime.MinValue;
            if (value is DateTime dt) return dt;
            if (DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtt)) return dtt;
            return fallback ?? DateTime.MinValue;
        }

        private static List<Models.TreeNode> CreateChildren(List<Models.GenericTree> recipe, int code)
        {
            var children = new List<Models.TreeNode>();
            if (recipe != null)
            {
                var kids = recipe.Where(o => o.Code != code && o.ParentCode == code && code > 0).OrderBy(o => o.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.TreeNode
                    {
                        title = k.Name,
                        key = k.Code,
                        icon = false,
                        children = CreateChildren(recipe, k.Code),
                        select = k.Flagged,
                        parenttitle = k.ParentName,
                        note = k.Note,
                        link = k.link
                    };
                    children.Add(child);
                }
            }
            return children;
        }
    }
}
