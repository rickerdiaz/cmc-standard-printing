using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CmcStandardPrinting.Domain.Ingredients;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IngredientController> _logger;

    public IngredientController(IConfiguration configuration, ILogger<IngredientController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codesite:int}/{codetrans:int}/{codesetprice:int}/{type:int}")]
    public ActionResult<IngredientListResponse> GetIngredients(
        int codesite,
        int codetrans,
        int codesetprice,
        int type,
        string? name = "",
        int skip = 0,
        int take = 10,
        int searchtype = 0,
        int category = -1,
        int sharetype = 0,
        int namefilter = 0,
        int isfulltext = 0,
        int sortby = 0,
        int status = -1)
    {
        try
        {
            var ingredients = new List<Ingredient>();
            var totalCount = 0;
            using var cmd = new SqlCommand { CommandTimeout = 1200 };
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Ingredients2]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codesetprice;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 250).Value = name ?? string.Empty;
            cmd.Parameters.Add("@SearchType", SqlDbType.Int).Value = searchtype;
            cmd.Parameters.Add("@Category", SqlDbType.Int).Value = category;
            cmd.Parameters.Add("@ShareType", SqlDbType.Int).Value = sharetype;
            cmd.Parameters.Add("@pimstatus", SqlDbType.Int).Value = status;
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@skip", SqlDbType.Int).Value = skip;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = take;
            if (isfulltext == 0) cmd.Parameters.Add("@NameFilter", SqlDbType.Int).Value = namefilter;
            cmd.Parameters.Add("@SortBy", SqlDbType.Int).Value = sortby;
            cmd.Parameters.Add("@IsFullText", SqlDbType.Int).Value = isfulltext;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read()) totalCount = GetInt(dr["Total"]);
            }

            if (dr.NextResult())
            {
                while (dr.Read())
                {
                    ingredients.Add(new Ingredient
                    {
                        CodeListe = GetInt(dr["CodeListe"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        CodeUser = GetInt(dr["CodeUser"]),
                        Type = GetInt(dr["Type"]),
                        Price = GetDbl(dr["Price"]),
                        UnitName = GetStr(dr["UnitName"]),
                        UnitMetric = GetStr(dr["UnitMetric"]),
                        UnitImperial = GetStr(dr["UnitImperial"]),
                        CodeUnit = GetInt(dr["CodeUnit"]),
                        CodeUnitMetric = GetInt(dr["CodeUnitMetric"]),
                        CodeUnitImperial = GetInt(dr["CodeUnitImperial"]),
                        CategoryName = GetStr(dr["CategoryName"]),
                        SourceName = GetStr(dr["SourceName"]),
                        SupplierName = GetStr(dr["SupplierName"]),
                        CodeBrand = GetInt(dr["CodeBrand"]),
                        BrandName = GetStr(dr["BrandName"]),
                        Wastage1 = GetInt(dr["Wastage1"]),
                        Wastage2 = GetInt(dr["Wastage2"]),
                        Wastage3 = GetInt(dr["Wastage3"]),
                        Wastage4 = GetInt(dr["Wastage4"]),
                        Wastage5 = GetInt(dr["Wastage5"]),
                        WastageTotal = GetInt(dr["WastageTotal"]),
                        Status = dr["Status"],
                        ImposedPrice = GetDbl(dr["ImposedPrice"]),
                        Constant = GetInt(dr["Constant"]),
                        Preparation = GetStr(dr["Preparation"]),
                        Allprice = DisplayAllPrice(GetInt(dr["CodeListe"]), codesetprice, type),
                        withTranslation = GetInt(dr["withTranslation"]),
                        isLocked = GetBool(dr["isLocked"]),
                        yieldIng = GetDbl(dr["Yield"])
                    });
                }
            }

            return Ok(new IngredientListResponse(ingredients, totalCount));
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get ingredients");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{codesetprice:int}/{codetype:int}/{codeliste:int}")]
    public ActionResult<List<Ingredient>> GetIngredientByListe(int codesite, int codetrans, int codesetprice, int codetype, int codeliste)
    {
        try
        {
            var ingredients = new List<Ingredient>();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_IngredientListe]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codesetprice;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeType", SqlDbType.Int).Value = codetype;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ingredients.Add(new Ingredient
                {
                    CodeListe = GetInt(dr["CodeListe"]),
                    Name = GetStr(dr["Name"]),
                    Number = GetStr(dr["Number"]),
                    CodeUser = GetInt(dr["CodeUser"]),
                    Type = GetInt(dr["Type"]),
                    Price = GetDbl(dr["Price"]),
                    UnitName = GetStr(dr["UnitName"]),
                    UnitMetric = GetStr(dr["UnitMetric"]),
                    UnitImperial = GetStr(dr["UnitImperial"]),
                    CodeUnit = GetInt(dr["CodeUnit"]),
                    CodeUnitMetric = GetInt(dr["CodeUnitMetric"]),
                    CodeUnitImperial = GetInt(dr["CodeUnitImperial"]),
                    CategoryName = GetStr(dr["CategoryName"]),
                    SourceName = GetStr(dr["SourceName"]),
                    SupplierName = GetStr(dr["SupplierName"]),
                    CodeBrand = GetInt(dr["CodeBrand"]),
                    BrandName = GetStr(dr["BrandName"]),
                    Wastage1 = GetInt(dr["Wastage1"]),
                    Wastage2 = GetInt(dr["Wastage2"]),
                    Wastage3 = GetInt(dr["Wastage3"]),
                    Wastage4 = GetInt(dr["Wastage4"]),
                    Wastage5 = GetInt(dr["Wastage5"]),
                    WastageTotal = GetInt(dr["WastageTotal"]),
                    Preparation = GetStr(dr["Preparation"])
                });
            }

            return Ok(ingredients);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get ingredient by liste");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveIngredient([FromBody] IngredientData data)
    {
        var response = new ResponseCallBack();
        SqlTransaction? trans = null;
        var resultCode = 0;
        try
        {
            if (data?.Info == null) throw new ArgumentNullException("invalid ingredient data");
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            var intCodeListe = GetInt(data.Info.CodeListe, -1);
            var intCodeSetPrice = GetInt(data.Info.CodeSetPrice, 0);
            var intCodeUnit = GetInt(data.Info.CodeUnit, -1);
            var strUnit = (data.Info.UnitName ?? string.Empty).Trim();
            var isNewUnit = Convert.ToBoolean(data.Info.IsNewUnit);
            var isNewIngredient = intCodeListe <= 0;
            var fltPrice = GetDbl(data.Info.Price);

            cmd.CommandTimeout = 120;
            cmd.Connection = cn;
            cmd.CommandText = "sp_egswListeUpdate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 2;
            cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = GetInt(data.Info.CodeSite);
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = GetInt(data.Info.CodeUser);
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = GetInt(data.Info.CodeTrans);
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = data.Info.Name ?? string.Empty;
            cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 20).Value = data.Info.Number ?? string.Empty;
            cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = GetInt(data.Info.CodeCategory);
            cmd.Parameters.Add("@sntWastage1", SqlDbType.SmallInt).Value = GetInt(data.Info.Wastage1);
            cmd.Parameters.Add("@sntWastage2", SqlDbType.SmallInt).Value = GetInt(data.Info.Wastage2);
            cmd.Parameters.Add("@sntWastage3", SqlDbType.SmallInt).Value = GetInt(data.Info.Wastage3);
            cmd.Parameters.Add("@sntWastage4", SqlDbType.SmallInt).Value = GetInt(data.Info.Wastage4);
            cmd.Parameters.Add("@nvcSubtitle", SqlDbType.NVarChar, 260).Value = string.Empty;
            cmd.Parameters.Add("@intSource", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@nvcRemark", SqlDbType.NVarChar, 250).Value = string.Empty;
            cmd.Parameters.Add("@fltYield", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intYieldUnit", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@sntPercent", SqlDbType.SmallInt).Value = 0;
            cmd.Parameters.Add("@fltSrQty", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intSrUnit", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 800).Value = string.Empty;
            cmd.Parameters.Add("@sdtDates", SqlDbType.SmallDateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@fltSrWeight", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@fltYield2", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intYieldUnit2", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@fltPortionSize", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intPortionSizeUnit", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@nvcPictureName", SqlDbType.NVarChar, 200).Value = ";;";
            cmd.Parameters.Add("@tntDefaultPicture", SqlDbType.TinyInt).Value = 0;
            cmd.Parameters.Add("@nvcNote", SqlDbType.NVarChar).Value = string.Empty;
            cmd.Parameters.Add("@nvcCoolingTime", SqlDbType.NVarChar, 25).Value = string.Empty;
            cmd.Parameters.Add("@nvcHeatingTime", SqlDbType.NVarChar, 25).Value = string.Empty;
            cmd.Parameters.Add("@nvcHeatingTemperature", SqlDbType.NVarChar, 25).Value = string.Empty;
            cmd.Parameters.Add("@nvcHeatingMode", SqlDbType.NVarChar, 25).Value = string.Empty;
            cmd.Parameters.Add("@nvcCCPDescription", SqlDbType.NVarChar, 255).Value = string.Empty;
            cmd.Parameters.Add("@nvcIngredients", SqlDbType.NVarChar, 2000).Value = string.Empty;
            cmd.Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700).Value = string.Empty;
            cmd.Parameters.Add("@nvcCookingTip", SqlDbType.NVarChar, 700).Value = string.Empty;
            cmd.Parameters.Add("@nvcRefinement", SqlDbType.NVarChar, 700).Value = string.Empty;
            cmd.Parameters.Add("@nvcStorage", SqlDbType.NVarChar, 700).Value = string.Empty;
            cmd.Parameters.Add("@nvcProductivity", SqlDbType.NVarChar, 700).Value = string.Empty;
            cmd.Parameters.Add("@bitProtected", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@intCodeLink", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@bitUse", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@intEGSRef", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@intEGSID", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@bitCompareByCodeSite", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 700).Value = string.Empty;
            cmd.Parameters.Add("@vchKeyField", SqlDbType.VarChar, 50).Value = string.Empty;
            cmd.Parameters.Add("@intOverwriteDescription", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@fltNetWeight", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@intTemplateCode", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@nvcNoteHeader", SqlDbType.NVarChar).Value = string.Empty;
            cmd.Parameters.Add("@bitAutoNum", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@nvcCodeStyle", SqlDbType.NVarChar).Value = string.Empty;
            cmd.Parameters.Add("@sStoringTime", SqlDbType.NVarChar, 100).Value = string.Empty;
            cmd.Parameters.Add("@sStoringTemp", SqlDbType.NVarChar, 100).Value = string.Empty;
            cmd.Parameters.Add("@bitOnline", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@nvcProtectedNote", SqlDbType.NVarChar, 200).Value = string.Empty;
            cmd.Parameters.Add("@nvcProtectedComment", SqlDbType.NVarChar, 200).Value = string.Empty;
            cmd.Parameters.Add("@bAllowDuplicates", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@fltPriceSmallPortion", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@fltPriceLargePortion", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@chrMethodFrmt", SqlDbType.Char, 1).Value = DBNull.Value;
            cmd.Parameters.Add("@nvcSubHeading", SqlDbType.NVarChar, 255).Value = string.Empty;
            cmd.Parameters.Add("@nvcFootNote1", SqlDbType.NVarChar, 4000).Value = string.Empty;
            cmd.Parameters.Add("@nvcFootNote2", SqlDbType.NVarChar, 4000).Value = string.Empty;
            cmd.Parameters.Add("@intTemplate", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Version", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@Standard", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Difficulty", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Budget", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@QuickAndEasy", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@ShowOff", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@ChefRecommended", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@nvcUPC", SqlDbType.NVarChar, 25).Value = string.Empty;
            cmd.Parameters.Add("@CostperServing", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@CostperRecipe", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("@LegacyNumber", SqlDbType.NVarChar).Value = string.Empty;
            cmd.Parameters.Add("@ServeWith", SqlDbType.NVarChar).Value = string.Empty;
            cmd.Parameters.Add("@PackagingCode", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@CertificationCode", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@OriginCode", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@TemperatureCode", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@InformationCode", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@intBrand", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@intSupplier", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@dteMenuCardDateFrom", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@dteMenuCardDateUntil", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@intMenuCardCodeSetPrice", SqlDbType.Int).Value = 0;
            var pCodeListeNew = cmd.Parameters.Add("@intCodeListeNew", SqlDbType.Int);
            pCodeListeNew.Direction = ParameterDirection.Output;
            var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            intCodeListe = GetInt(pCodeListeNew.Value, -1);
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new Exception($"[{resultCode}] Ingredient create/update failed");
            if (intCodeListe <= 0) throw new Exception($"[{resultCode}] Ingredient was not created");

            if (intCodeUnit < 0 && !string.IsNullOrEmpty(strUnit))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_EgswUnitGetList";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@tntType", SqlDbType.TinyInt).Value = 2;
                cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = GetInt(data.Info.CodeTrans);
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = GetInt(data.Info.CodeSite);
                cmd.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@Status", SqlDbType.TinyInt).Value = 255;
                cmd.Parameters.Add("@vchName", SqlDbType.NVarChar, 32).Value = strUnit;
                cmd.Parameters.Add("@IsYield", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@IsIngredient", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@IsStock", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@IsTransportation", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@IsPackaging", SqlDbType.Int).Value = DBNull.Value;
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read()) intCodeUnit = GetInt(dr["Code"], -1);
                }

                if (intCodeUnit < 0)
                {
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = "sp_EgswUnitUpdate";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    var retUnit = cmd.Parameters.Add("@retval", SqlDbType.Int);
                    cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = GetInt(data.Info.CodeUser);
                    cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = GetInt(data.Info.CodeSite);
                    var pUnitCode = cmd.Parameters.Add("@intCode", SqlDbType.Int);
                    pUnitCode.Value = intCodeUnit;
                    pUnitCode.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.Add("@nvcNameDef", SqlDbType.NVarChar, 32).Value = strUnit;
                    cmd.Parameters.Add("@nvcNamePlural", SqlDbType.NVarChar, 32).Value = strUnit;
                    cmd.Parameters.Add("@nvcNameDisp", SqlDbType.NVarChar, 32).Value = strUnit;
                    cmd.Parameters.Add("@nvcAutoConversion", SqlDbType.NVarChar, 500).Value = strUnit;
                    cmd.Parameters.Add("@IsBasic", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@IsStock", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@IsPackaging", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@IsTranspo", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@IsIngredient", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@IsYield", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@IsServing", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@fltFactor", SqlDbType.Float).Value = 1;
                    cmd.Parameters.Add("@intTypeMain", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@IsMetric", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@nvcFormat", SqlDbType.NVarChar, 15).Value = "#,##0.000";
                    cmd.Parameters.Add("@IsAdded", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@intPosition", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = 1;
                    cmd.Parameters.Add("@bitUseMakes", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = 1;
                    cmd.Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = "(" + GetInt(data.Info.CodeSite) + ")";
                    retUnit.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    intCodeUnit = GetInt(pUnitCode.Value, -1);
                    resultCode = GetInt(retUnit.Value, -1);
                    if (resultCode != 0) throw new Exception($"[{resultCode}] Update ingredient unit failed");

                    if (isNewUnit && intCodeUnit >= 0)
                    {
                        cmd.CommandText = "sp_EgswItemTranslationUpdate";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeUnit;
                        cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = strUnit;
                        cmd.Parameters.Add("@nvcName2", SqlDbType.NVarChar, 150).Value = strUnit;
                        cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = GetInt(data.Info.CodeTrans);
                        cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = GetInt(data.Info.CodeSite);
                        cmd.Parameters.Add("@tntListType", SqlDbType.Int).Value = 3;
                        cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = GetInt(data.Info.CodeUser);
                        cmd.Parameters.Add("@tntType", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@nvcPlural", SqlDbType.NVarChar, 150).Value = string.Empty;
                        var retTr = cmd.Parameters.Add("@retval", SqlDbType.Int);
                        retTr.Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(retTr.Value, -1);
                        if (resultCode != 0) throw new Exception($"[{resultCode}] Update unit translation failed");
                    }
                }
            }

            if ((isNewIngredient || isNewUnit) && intCodeUnit > 0)
            {
                var intPosition = 0;
                cmd.CommandText = "SELECT @Pos = ISNULL(MAX(Position), 0) FROM dbo.EgswListeSetPrice WHERE CodeListe = @CodeListe";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                var pPos = cmd.Parameters.Add("@Pos", SqlDbType.Int);
                pPos.Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe;
                cmd.ExecuteNonQuery();
                intPosition = GetInt(pPos.Value, 0);

                cmd.CommandText = "sp_egswListeSetPriceUpdate";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@intID", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice;
                cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe;
                cmd.Parameters.Add("@intCodeUnit", SqlDbType.Int).Value = intCodeUnit;
                cmd.Parameters.Add("@fltPrice", SqlDbType.Float).Value = fltPrice;
                cmd.Parameters.Add("@intPosition", SqlDbType.Int).Value = intPosition + 1;
                cmd.Parameters.Add("@fltRatio", SqlDbType.Float).Value = 0;
                cmd.Parameters.Add("@fltRatioNut", SqlDbType.Float).Value = 0;
                cmd.Parameters.Add("@vchFnc", SqlDbType.VarChar, 20).Value = "UPDATEPRICE";
                cmd.Parameters.Add("@intTax", SqlDbType.Int).Value = 0;
                var retSP = cmd.Parameters.Add("@retval", SqlDbType.Int);
                retSP.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(retSP.Value, -1);
                if (resultCode != 0) throw new Exception($"[{resultCode}] Add ingredient setprice failed");
            }

            if (data.Sharing != null && data.Sharing.Count > 0)
            {
                cmd.CommandText = "DELETE FROM EgswSharing WHERE Code=@CodeListe AND CodeEgswTable=50";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe;
                cmd.ExecuteNonQuery();

                var seed = 1;
                foreach (var sh in data.Sharing)
                {
                    cmd.CommandText = "INSERT INTO [dbo].[EgswSharing] ([Code],[CodeUserOwner],[CodeUserSharedTo],[Type],[CodeEgswTable],[Position],[Status],[IsGlobal]) VALUES (@CodeListe,@CodeUserOwner,@CodeSite,1,50,@Position,1,@Global)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe;
                    cmd.Parameters.Add("@CodeUserOwner", SqlDbType.Int).Value = data.Info.CodeSite;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = sh.Code;
                    cmd.Parameters.Add("@Position", SqlDbType.Int).Value = seed;
                    cmd.Parameters.Add("@Global", SqlDbType.Bit).Value = Convert.ToBoolean(sh.Value);
                    cmd.ExecuteNonQuery();
                    seed++;
                }
            }

            trans.Commit();
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = intCodeListe;
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
            response.Message = "Save ingredient failed";
            _logger.LogError(ex, "Failed to save ingredient");
            return StatusCode(500, response);
        }

        return Ok(response);
    }

    [HttpGet("getprice/{codeliste:int}/{codeunit:int}/{codesetprice:int}")]
    public ActionResult<List<IngredientOnePrice>> GetIngredientOnePrice(int codeliste, int codeunit, int codesetprice)
    {
        try
        {
            var result = new List<IngredientOnePrice>();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_EgswListeSetPriceGetOnePrice]";
            cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@intCodeUnit", SqlDbType.Int).Value = codeunit;
            cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = codesetprice;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                result.Add(new IngredientOnePrice
                {
                    Code = GetInt(dr["CodeListe"]),
                    CodeUnit = dr["Unit"],
                    Position = dr["Position"],
                    Price = dr["Price"],
                    CodeSetPrice = dr["CodeSetPrice"]
                });
            }

            return Ok(result);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get ingredient price");
            return Problem(title: ex.ToString(), statusCode: 500);
        }
    }

    private string DisplayAllPrice(int codeListe, int codeSetPrice, int type)
    {
        var allprice = string.Empty;
        using var cmd = new SqlCommand();
        using var cn = new SqlConnection(ConnectionString);
        try
        {
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_IngredientAllSetPrice]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeListe;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codeSetPrice;
            cmd.Parameters.Add("@mainType", SqlDbType.Int).Value = type;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) allprice += Convert.ToString(dr["Allprice"]) + ",";
        }
        catch
        {
        }

        return allprice;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (int.TryParse(Convert.ToString(value), out var i)) return i;
        try
        {
            return Convert.ToInt32(value);
        }
        catch
        {
            return fallback;
        }
    }

    private static double GetDbl(object? value, double fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (double.TryParse(Convert.ToString(value), out var d)) return d;
        try
        {
            return Convert.ToDouble(value);
        }
        catch
        {
            return fallback;
        }
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (bool.TryParse(Convert.ToString(value), out var b)) return b;
        try
        {
            return Convert.ToInt32(value) != 0;
        }
        catch
        {
            return false;
        }
    }

    private static string GetStr(object? value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
}
