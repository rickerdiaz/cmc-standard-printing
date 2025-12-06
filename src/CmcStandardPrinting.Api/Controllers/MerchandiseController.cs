using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Merchandises;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MerchandiseController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MerchandiseController> _logger;

    public MerchandiseController(IConfiguration configuration, ILogger<MerchandiseController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("purchasingsetprice/{merchCode:int}/{rayonCode:int}")]
    public ActionResult<DataTable> GetPurchasingSetPrice(int merchCode, int rayonCode)
    {
        var dt = new DataTable();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_PurchasingSetPrice]";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@merchCode", SqlDbType.Int).Value = merchCode;
            cmd.Parameters.Add("@rayonCode", SqlDbType.Int).Value = rayonCode;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return Ok(dt);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid purchasing set price request");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading purchasing set price");
            return StatusCode(500);
        }
    }

    [HttpGet("history/{codeliste:int}/{codetrans:int}")]
    public ActionResult<RecipeHistoryResponse> GetRecipeHistorys(int codeliste, int codetrans, int codeuser = -1, int skip = 0, int take = 10, int datefilter = 0, string datefrom = "", string dateto = "", int ActionType = 0)
    {
        var histories = new List<RecipeHistory>();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_EgswListeGetHistoryLogsMerchandise";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = codeuser;
            cmd.Parameters.Add("@intFieldCode", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@bitFirstFewRecords", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@intDateFilterOption", SqlDbType.Int).Value = datefilter;
            cmd.Parameters.Add("@dteDateFrom", SqlDbType.DateTime).Value = string.IsNullOrEmpty(datefrom) ? (object)DBNull.Value : GetDate(datefrom, DateTime.Now);
            cmd.Parameters.Add("@dteDateTo", SqlDbType.DateTime).Value = string.IsNullOrEmpty(dateto) ? (object)DBNull.Value : GetDate(dateto, DateTime.Now);
            cmd.Parameters.Add("@intActionType", SqlDbType.Int).Value = ActionType;
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                histories.Add(new RecipeHistory
                {
                    DateAudit = GetStr(dr["DateAudit"]),
                    FieldName = GetStr(dr["FieldName"]),
                    Time = GetStr(dr["Time"]),
                    FieldCode = GetStr(dr["FieldCode"]),
                    Previous = GetStr(dr["Previous"]),
                    HNew = GetStr(dr["New"]),
                    User = GetStr(dr["User"]),
                    AuditType = GetStr(dr["AuditType"]),
                    CodeListe = GetStr(dr["CodeListe"]),
                    CodeUser = GetStr(dr["CodeUser"]),
                    IsCode = GetStr(dr["IsCode"])
                });
            }

            var totalCount = histories.Count;
            take = take > totalCount + 1 ? totalCount : take;
            take = take <= 0 ? 1 : take;
            var totalPages = (int)Math.Ceiling((double)totalCount / take);
            skip = skip > totalPages ? totalPages : skip;
            skip = skip < 1 ? 0 : skip;
            var resp = new RecipeHistoryResponse(histories.Skip(take * skip).Take(take).ToList(), totalCount);
            return Ok(resp);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid merchandise history request");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading merchandise history");
            return StatusCode(500);
        }
    }

    [HttpGet("{codeliste:int}/{codesite:int}/{codetrans:int}/{codesetprice:int}/{codenutrientset:int}")]
    public ActionResult<MerchandiseData> GetMerchandise(int codeliste, int codesite, int codetrans, int codesetprice, int codenutrientset)
    {
        var merchandiseData = new MerchandiseData();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_MerchandiseInfo]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codesetprice;
            cmd.Parameters.Add("@CodeNutrientSet", SqlDbType.Int).Value = codenutrientset;
            cn.Open();
            using (var dr = cmd.ExecuteReader())
            {
                if (dr.HasRows)
                {
                    var info = new Merchandise();
                    while (dr.Read())
                    {
                        info.Name = GetStr(dr["Name"]);
                        info.Number = GetStr(dr["Number"]);
                        info.UPC = GetStr(dr["UPC"]);
                        info.CodeListe = GetInt(dr["CodeListe"]);
                        info.CodeBrand = GetInt(dr["CodeBrand"]);
                        info.Brand = GetStr(dr["Brand"]);
                        info.CodeCategory = GetInt(dr["CodeCategory"]);
                        info.Category = GetStr(dr["Category"]);
                        info.CodeSupplier = GetInt(dr["CodeSupplier"]);
                        info.Supplier = GetStr(dr["Supplier"]);
                        info.CodeTrans = GetInt(dr["CodeTrans"]);
                        info.DefaultPicture = GetInt(dr["DefaultPicture"]);
                        info.Date = GetDate(dr["Date"]).ToString("MM/dd/yyyy");
                        info.ModifiedDate = GetDate(dr["ModifiedDate"]).ToString("MM/dd/yyyy");
                        info.Wastage1 = GetInt(dr["Wastage1"]);
                        info.Wastage2 = GetInt(dr["Wastage2"]);
                        info.Wastage3 = GetInt(dr["Wastage3"]);
                        info.Wastage4 = GetInt(dr["Wastage4"]);
                        info.Wastage5 = GetInt(dr["Wastage5"]);
                        info.Picture = GetStr(dr["PictureName"]).Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                        info.CustomTempAttachments = new List<MerchandiseAttachment>();
                        info.CustomTempPictures = new List<string>();
                        info.InUse = GetBool(dr["InUse"]);
                        info.CodeLink = GetStr(dr["CodeLink"]);
                        info.cGlobal = GetBool(dr["Global"]);
                        info.AllergenApproved = GetBool(dr["AllergenApproved"]);
                        info.LinkNutrient = GetStr(dr["LinkNutrient"]);
                        info.CodeNutrientSet = GetInt(dr["CodeNutrientSet"]);
                        info.CodeCountry = GetInt(dr["CodeCountry"]);
                        info.Country = GetStr(dr["Country"]);
                    }
                    merchandiseData.Info = info;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var priceList = new List<MerchandisePrice>();
                    while (dr.Read())
                    {
                        priceList.Add(new MerchandisePrice
                        {
                            Id = GetInt(dr["Id"]),
                            Unit = GetStr(dr["Unit"]),
                            CodeUnit = GetInt(dr["CodeUnit"]),
                            Price = GetDbl(dr["Price"]),
                            Ratio = GetDbl(dr["Ratio"]),
                            TaxCode = GetInt(dr["Tax"]),
                            TaxValue = GetDbl(dr["TaxValue"]),
                            Position = GetInt(dr["Position"]),
                            IsUsed = GetBool(dr["IsUsed"]),
                            CodeSetPrice = GetInt(dr["CodeSetPrice"])
                        });
                    }
                    merchandiseData.Price = priceList;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var keywords = new List<GenericTree>();
                    while (dr.Read())
                    {
                        keywords.Add(new GenericTree
                        {
                            Name = GetStr(dr["Name"]),
                            Flagged = GetBool(dr["Flagged"]),
                            Code = GetInt(dr["Code"]),
                            ParentCode = GetInt(dr["ParentCode"])
                        });
                    }
                    merchandiseData.Keywords = keywords;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var nutrients = new List<RecipeNutrition>();
                    while (dr.Read())
                    {
                        nutrients.Add(new RecipeNutrition
                        {
                            Name = GetStr(dr["Name"]),
                            Nutr_No = GetInt(dr["Nutr_No"]),
                            Position = GetInt(dr["Position"]),
                            TagName = GetStr(dr["TagName"]),
                            Value = GetDbl(dr["Value"]),
                            Imposed = GetInt(dr["Imposed"]),
                            Percent = GetInt(dr["Percent"]),
                            Format = GetStr(dr["Format"]),
                            Unit = GetStr(dr["Unit"]),
                            CodeNutrientSet = GetInt(dr["CodeNutrientSet"])
                        });
                    }
                    merchandiseData.Nutrient = nutrients;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var translations = new List<MerchandiseTranslation>();
                    while (dr.Read())
                    {
                        translations.Add(new MerchandiseTranslation
                        {
                            Id = GetInt(dr["Id"]),
                            TranslationCode = GetInt(dr["CodeTrans"]),
                            TranslationName = GetStr(dr["TranslationName"]),
                            CodeDictionary = GetInt(dr["CodeDictionary"]),
                            Name = GetStr(dr["Name"]),
                            Ingredients = GetStr(dr["Ingredients"]),
                            Preparation = GetStr(dr["Preparation"]),
                            CookingTip = GetStr(dr["CookingTip"]),
                            Refinement = GetStr(dr["Refinement"]),
                            SpecificDetermination = GetStr(dr["SpecificDetermination"]),
                            Storage = GetStr(dr["Storage"]),
                            Productivity = GetStr(dr["Productivity"]),
                            Description = GetStr(dr["Description"]),
                            PrefixCode = GetStr(dr["PrefixCode"]),
                            PrefixName = GetStr(dr["PrefixName"]),
                            Gender = GetBool(dr["IsFemale"]) == false ? "Masculine" : "Feminine",
                            IsGenderSensitive = GetBool(dr["IsGenderSensitive"])
                        });
                    }
                    merchandiseData.Translation = translations;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var sharings = new List<GenericTree>();
                    while (dr.Read())
                    {
                        sharings.Add(new GenericTree
                        {
                            Code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            ParentCode = GetInt(dr["ParentCode"]),
                            ParentName = GetStr(dr["ParentName"]),
                            Flagged = GetBool(dr["Flagged"]),
                            Type = GetInt(dr["Type"])
                        });
                    }
                    var sharingdata = new List<TreeNode>();
                    var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
                    foreach (var p in parents)
                    {
                        var parent = new TreeNode
                        {
                            title = p.Name,
                            key = p.Code,
                            icon = false,
                            children = CreateChildrenSharing(sharings, p.Code, codesite),
                            select = p.Flagged,
                            selected = p.Flagged,
                            parenttitle = p.ParentName,
                            groupLevel = GroupLevel.Property
                        };
                        sharingdata.Add(parent);
                    }
                    merchandiseData.Sharing = sharingdata;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var allergens = new List<ListeAllergen>();
                    while (dr.Read())
                    {
                        allergens.Add(new ListeAllergen
                        {
                            CodeAllergen = GetInt(dr["CodeAllergen"]),
                            AllergenName = GetStr(dr["Allergen"]),
                            Contain = GetBool(dr["Contain"]),
                            NonAllergen = GetBool(dr["NonAllergen"]),
                            Trace = GetBool(dr["Trace"]),
                            Derived = GetBool(dr["Derived"]),
                            Hidden = GetBool(dr["Hidden"]),
                            PictureName = GetStr(dr["PictureName"]),
                            Complete = GetStr(dr["Complete"]),
                            SwissLaw = GetStr(dr["SwLaw"]),
                            EULaw = GetStr(dr["EuLaw"])
                        });
                    }
                    merchandiseData.Allergen = allergens;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var histories = new List<MerchandiseHistory>();
                    while (dr.Read())
                    {
                        histories.Add(new MerchandiseHistory
                        {
                            DateAudit = GetStr(dr["DateAudit"]),
                            FieldName = GetStr(dr["FieldName"]),
                            Time = GetStr(dr["Time"]),
                            FieldCode = GetStr(dr["FieldCode"]),
                            Previous = GetStr(dr["Previous"]),
                            HNew = GetStr(dr["New"]),
                            User = GetStr(dr["User"]),
                            AuditType = GetStr(dr["AuditType"]),
                            CodeListe = GetStr(dr["CodeListe"]),
                            CodeUser = GetStr(dr["CodeUser"]),
                            IsCode = GetStr(dr["IsCode"])
                        });
                    }
                    merchandiseData.History = histories;
                }
            }
            return Ok(merchandiseData);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid merchandise request");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading merchandise");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveMerchandise([FromBody] MerchandiseData data)
    {
        var response = new ResponseCallBack();
        SqlTransaction? _trans = null;
        int resultCode = 0;
        string pictures = string.Empty;
        var arrDAMCode = new ArrayList();
        try
        {
            if (data?.Info == null) return BadRequest();
            if (data.Info.Picture != null)
            {
                var arrPictures = data.Info.Picture.ToArray();
                for (int ctr = 0; ctr < arrPictures.Length; ctr++)
                {
                    var pic = arrPictures[ctr];
                    if (!string.IsNullOrWhiteSpace(pic) && pic.Contains("| DAM"))
                    {
                        pic = pic.Substring(0, pic.LastIndexOf("|", StringComparison.Ordinal)).Trim();
                        arrDAMCode.Add(pic.Substring(pic.IndexOf("|", StringComparison.Ordinal) + 1));
                        pic = pic.Substring(0, pic.LastIndexOf("|", StringComparison.Ordinal)).Trim();
                        arrPictures[ctr] = $"P{DateTime.Now:MMddyyHHmmss}_{ctr}{Path.GetExtension(pic)}";
                    }
                }
                pictures = string.Join(";", arrPictures);
            }

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            int intCodeListe = -1;
            int intCodeUser = GetInt(data.Info.CodeModifiedBy);
            cmd.CommandTimeout = 120;
            cmd.Connection = cn;
            cmd.CommandText = "sp_egswListeUpdate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = data.Info.CodeListe;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Info.CodeSite;
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Info.CodeUser;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.Info.CodeTrans;
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 2;
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = Common.ReplaceSpecialCharacters(data.Info.Name);
            cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 20).Value = Common.ReplaceSpecialCharacters(data.Info.Number);
            cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = data.Info.CodeCategory;
            cmd.Parameters.Add("@intSource", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@nvcRemark", SqlDbType.NVarChar, 250).Value = string.Empty;
            cmd.Parameters.Add("@fltYield", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intYieldUnit", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@sntPercent", SqlDbType.SmallInt).Value = 100;
            cmd.Parameters.Add("@fltSrQty", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intSrUnit", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@sdtDates", SqlDbType.SmallDateTime).Value = GetDate(data.Info.Date, DateTime.Now);
            cmd.Parameters.Add("@fltSrWeight", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@fltYield2", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intYieldUnit2", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@fltPortionSize", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intPortionSizeUnit", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@CodeCountry", SqlDbType.Int).Value = data.Info.CodeCountry;
            cmd.Parameters.Add("@nvcPictureName", SqlDbType.NVarChar, 200).Value = pictures;
            cmd.Parameters.Add("@tntDefaultPicture", SqlDbType.TinyInt).Value = data.Info.DefaultPicture;
            cmd.Parameters.Add("@nvcHACCP", SqlDbType.NVarChar, 4000).Value = string.Empty;
            cmd.Parameters.Add("@nvcNote", SqlDbType.NVarChar).Value = Common.ReplaceSpecialCharacters(string.Empty);
            var currentTranslation = data.Translation?.FirstOrDefault(t => t.TranslationCode == data.Info.CodeTrans) ?? new MerchandiseTranslation();
            cmd.Parameters.Add("@nvcIngredients", SqlDbType.NVarChar, 2000).Value = Common.ReplaceSpecialCharacters(currentTranslation.Ingredients);
            cmd.Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700).Value = Common.ReplaceSpecialCharacters(currentTranslation.Preparation);
            cmd.Parameters.Add("@nvcCookingTip", SqlDbType.NVarChar, 700).Value = Common.ReplaceSpecialCharacters(currentTranslation.CookingTip);
            cmd.Parameters.Add("@nvcRefinement", SqlDbType.NVarChar, 700).Value = Common.ReplaceSpecialCharacters(currentTranslation.Refinement);
            cmd.Parameters.Add("@nvcStorage", SqlDbType.NVarChar, 700).Value = Common.ReplaceSpecialCharacters(currentTranslation.Storage);
            cmd.Parameters.Add("@nvcProductivity", SqlDbType.NVarChar, 700).Value = Common.ReplaceSpecialCharacters(currentTranslation.Productivity);
            cmd.Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 800).Value = Common.ReplaceSpecialCharacters(currentTranslation.Description);
            cmd.Parameters.Add("@bitProtected", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@intCodeLink", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.cGlobal;
            cmd.Parameters.Add("@bitUse", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@intEGSRef", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@intEGSID", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@bitCompareByCodeSite", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 700).Value = string.Empty;
            cmd.Parameters.Add("@vchKeyField", SqlDbType.VarChar, 50).Value = string.Empty;
            cmd.Parameters.Add("@intOverwriteDescription", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@fltNetWeight", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intTemplateCode", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@nvcNoteHeader", SqlDbType.NVarChar).Value = Common.ReplaceSpecialCharacters(string.Empty);
            cmd.Parameters.Add("@bitAutoNum", SqlDbType.Bit).Value = data.Info.CodeListe > 0;
            cmd.Parameters.Add("@nvcCodeStyle", SqlDbType.NVarChar).Value = Common.ReplaceSpecialCharacters(string.Empty);
            cmd.Parameters.Add("@bitOnline", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@nvcProtectedNote", SqlDbType.NVarChar, 200).Value = Common.ReplaceSpecialCharacters(string.Empty);
            cmd.Parameters.Add("@nvcProtectedComment", SqlDbType.NVarChar, 200).Value = Common.ReplaceSpecialCharacters(string.Empty);
            cmd.Parameters.Add("@bAllowDuplicates", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@fltPriceSmallPortion", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@fltPriceLargePortion", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@nvcSubHeading", SqlDbType.NVarChar, 255).Value = Common.ReplaceSpecialCharacters(string.Empty);
            cmd.Parameters.Add("@intTemplate", SqlDbType.Int).Value = 1000;
            cmd.Parameters.Add("@Version", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@ShowOff", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@ChefRecommended", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@nvcUPC", SqlDbType.NVarChar, 25).Value = Common.ReplaceSpecialCharacters(data.Info.UPC);
            cmd.Parameters.Add("@CostperServing", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@CostperRecipe", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@LegacyNumber", SqlDbType.NVarChar).Value = string.Empty;
            cmd.Parameters.Add("@ServeWith", SqlDbType.NVarChar).Value = string.Empty;
            cmd.Parameters.Add("@intBrand", SqlDbType.Int).Value = data.Info.CodeBrand;
            cmd.Parameters.Add("@intSupplier", SqlDbType.Int).Value = data.Info.CodeSupplier;
            cmd.Parameters.Add("@sntWastage1", SqlDbType.SmallInt).Value = data.Info.Wastage1;
            cmd.Parameters.Add("@sntWastage2", SqlDbType.SmallInt).Value = data.Info.Wastage2;
            cmd.Parameters.Add("@sntWastage3", SqlDbType.SmallInt).Value = data.Info.Wastage3;
            cmd.Parameters.Add("@sntWastage4", SqlDbType.SmallInt).Value = data.Info.Wastage4;
            cmd.Parameters.Add("@dteMenuCardDateFrom", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@dteMenuCardDateUntil", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@intMenuCardCodeSetPrice", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@bitAllergenApproved", SqlDbType.Bit).Value = data.Info.AllergenApproved;
            cmd.Parameters.Add("@sdtTested", SqlDbType.SmallDateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@intCodeListeNew", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            cn.Open();
            _trans = cn.BeginTransaction();
            cmd.Transaction = _trans;
            cmd.ExecuteNonQuery();
            intCodeListe = GetInt(cmd.Parameters["@intCodeListeNew"].Value, -1);
            resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
            if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Merchandise update failed"));
            _trans.Commit();

            // History
            cmd.CommandText = "sp_egswListeHistoryUpdate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@intCodeUserID", SqlDbType.Int).Value = intCodeUser;
            cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe;
            cmd.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
            if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Update list history failed"));

            response.Code = 0;
            response.ReturnValue = intCodeListe;
            response.Message = Common.ReplaceSpecialCharacters(data.Info.Name) + " successfully saved.";
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while saving merchandise");
            try { _trans?.Rollback(); } catch { }
            if (resultCode == 0) resultCode = 500;
            return StatusCode(500, Fail(response, resultCode, "Save merchandise failed"));
        }
    }

    [HttpPost("applywastage")]
    public ActionResult<ResponseCallBack> ApplyWastage([FromBody] MerchandiseData data)
    {
        var response = new ResponseCallBack();
        SqlTransaction? _trans = null;
        int resultCode = 0;
        try
        {
            if (data?.Info == null) return BadRequest();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.CommandTimeout = 120;
            cmd.Connection = cn;
            cmd.CommandText = "[dbo].[SP_EgswListeUpdateWastage]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = data.Info.CodeListe;
            cmd.Parameters.Add("@intWastage1", SqlDbType.SmallInt).Value = data.Info.Wastage1;
            cmd.Parameters.Add("@intWastage2", SqlDbType.SmallInt).Value = data.Info.Wastage2;
            cmd.Parameters.Add("@intWastage3", SqlDbType.SmallInt).Value = data.Info.Wastage3;
            cmd.Parameters.Add("@intWastage4", SqlDbType.SmallInt).Value = data.Info.Wastage4;
            cmd.Parameters.Add("@intWastage5", SqlDbType.SmallInt).Value = data.Info.Wastage5;
            cn.Open();
            _trans = cn.BeginTransaction();
            cmd.Transaction = _trans;
            cmd.ExecuteNonQuery();
            _trans.Commit();
            response.Code = 0; response.ReturnValue = data.Info.CodeListe; response.Message = "Wastage successfully applied."; response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while applying wastage");
            try { _trans?.Rollback(); } catch { }
            if (resultCode == 0) resultCode = 500;
            return StatusCode(500, Fail(response, resultCode, "Applying wastage failed."));
        }
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> sharingdata, int code, int codesite)
    {
        var children = new List<TreeNode>();
        if (sharingdata != null)
        {
            var kids = sharingdata.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
            foreach (var k in kids)
            {
                var child = new TreeNode
                {
                    title = k.Name,
                    key = k.Code,
                    icon = false,
                    children = null,
                    select = false,
                    selected = k.Flagged,
                    parenttitle = k.ParentName,
                    note = k.Global,
                    groupLevel = GroupLevel.Site,
                    unselectable = k.Code == codesite
                };
                children.Add(child);
            }
        }
        return children;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (int.TryParse(Convert.ToString(value), out var i)) return i;
        try { return Convert.ToInt32(value); } catch { return fallback; }
    }

    private static double GetDbl(object? value, double fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (double.TryParse(Convert.ToString(value), out var d)) return d;
        try { return Convert.ToDouble(value); } catch { return fallback; }
    }

    private static string GetStr(object? value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value), out var bb)) return bb;
        return false;
    }

    private static DateTime GetDate(object? value, DateTime? fallback = null)
    {
        if (value == null || value == DBNull.Value) return fallback ?? DateTime.MinValue;
        if (DateTime.TryParse(Convert.ToString(value), out var d)) return d;
        return fallback ?? DateTime.MinValue;
    }

    private static DateTime GetDate(string? value, DateTime fallback)
    {
        if (DateTime.TryParse(value, out var d)) return d;
        return fallback;
    }

    private static ResponseCallBack Fail(ResponseCallBack r, int code, string message)
    {
        r.Code = code;
        r.Message = message;
        r.Status = false;
        return r;
    }
}
