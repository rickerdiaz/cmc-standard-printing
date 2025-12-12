using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class SearchController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IConfiguration configuration, ILogger<SearchController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("api/search/recipe")]
    public ActionResult<object> SearchRecipe(
        int codeuser = 0,
        int codesite = -1,
        int codetrans = 0,
        int codesetprice = -1,
        int codeset = 0,
        int namefilter = 0,
        string name = "",
        bool treeview = false,
        long codeliste = -1,
        bool getRecipeLink = false)
    {
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandTimeout = 500;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_SEARCH_RECIPE]";
            cmd.Parameters.Add("@listview", SqlDbType.Int).Value = 2;
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = 8;
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = codeuser;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = codesetprice;
            cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = codeset;
            cmd.Parameters.Add("@codeliste", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@intNameFilter", SqlDbType.Int).Value = namefilter;
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = name ?? string.Empty;
            cmd.Parameters.Add("@getRecipeLinks", SqlDbType.Bit).Value = getRecipeLink;

            cn.Open();
            using var dr = cmd.ExecuteReader();
            dr.NextResult();
            dr.NextResult();

            if (!dr.HasRows) return Ok(Array.Empty<object>());

            if (!treeview)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        PrimaryBrand = GetStr(dr["PrimaryBrand"]),
                        SecondaryBrand = GetStr(dr["SecondaryBrand"]),
                        RecipeStatus = GetStr(dr["RecipeStatus"]),
                        Image = GetBool(dr["ImageDisplay"]),
                        PictureName = GetStr(dr["PictureName"]),
                        Nutrition = GetBool(dr["DisplayNutrition"]),
                        DateCreated = GetDate(dr["Dates"]).ToString(CultureInfo.InvariantCulture),
                        Category = GetStr(dr["Category"]),
                        Owner = GetStr(dr["Owner"]),
                        Yield = GetInt(dr["Yield"]),
                        YieldFormat = GetStr(dr["YieldFormat"]),
                        YieldName = GetStr(dr["YieldName"]),
                        Source = GetStr(dr["Source"]),
                        Unit = GetStr(dr["Yield"]),
                        Status = GetStr(dr["Status"]),
                        CalcPrice = GetDbl(dr["CalcPrice"]),
                        ImposedPrice = GetDbl(dr["ImposedPrice"]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        FoodCost = GetDbl(dr["FoodCost"]),
                        FoodCostPercent = GetDbl(dr["FoodCostPercent"]),
                        GrossMargin = GetDbl(dr["GrossMargin"]),
                        GrossMarginPercent = GetDbl(dr["GrossMarginPercent"]),
                        NetMargin = GetDbl(dr["NetMargin"]),
                        NetMarginPercent = GetDbl(dr["NetMarginPercent"]),
                        ImposedSellingPriceWOTax = GetDbl(dr["ImposedSellingPriceWOTax"]),
                        ImposedSellingPriceWTax = GetDbl(dr["ImposedSellingPriceWTax"]),
                        PimFlag = GetInt(dr["PIMFlag"]),
                        DateTested = GetDate(dr["DateTested"]),
                        WithTranslation = GetInt(dr["WithTranslation"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }

                return Ok(new ResponseGenericSearch(lists, lists.Count));
            }

            var recipes = new List<GenericTree>();
            while (dr.Read())
            {
                recipes.Add(new GenericTree
                {
                    Code = GetInt(dr["Code"]),
                    Name = GetStr(dr["Name"]),
                    Number = GetStr(dr["Number"]),
                    ParentCode = GetInt(dr["ParentCode"]),
                    Link = GetStr(dr["Link"]),
                    Flagged = GetBool(dr["Flag"]),
                    Note = GetStr(dr["Note"])
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
                        Title = (!string.IsNullOrEmpty(p.Number) ? $"[{p.Number}] - " : string.Empty) + p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildren(recipes, p.Code),
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
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchRecipe failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("api/search/recipe")]
    public ActionResult<object> SearchRecipeData([FromBody] Searchee data)
    {
        try
        {
            int totalCount = 0;
            var primarybrands = new ArrayList();
            var unwantedprimarybrands = new ArrayList();
            var secondarybrands = new ArrayList();
            var unwantedsecondarybrands = new ArrayList();
            var keywords = new ArrayList();
            var unwantedkeywords = new ArrayList();
            var allergens = new ArrayList();
            var unwantedallergens = new ArrayList();
            var kiosks = new ArrayList();
            var defformat = new List<string>();

            if (data?.PrimaryBrands != null)
                foreach (var x in data.PrimaryBrands.ToArray()) primarybrands.Add(x.Key);
            if (data?.UnwantedPrimaryBrands != null)
                foreach (var x in data.UnwantedPrimaryBrands.ToArray()) unwantedprimarybrands.Add(x.Key);
            if (data?.SecondaryBrands != null)
                foreach (var x in data.SecondaryBrands.ToArray()) secondarybrands.Add(x.Key);
            if (data?.UnwantedSecondaryBrands != null)
                foreach (var x in data.UnwantedSecondaryBrands.ToArray()) unwantedsecondarybrands.Add(x.Key);
            if (data?.Keywords != null)
                foreach (var x in data.Keywords.ToArray()) keywords.Add(x.Key);
            if (data?.UnwantedKeywords != null)
                foreach (var x in data.UnwantedKeywords.ToArray()) unwantedkeywords.Add(x.Key);
            if (data?.Allergens != null)
                foreach (var x in data.Allergens.ToArray()) allergens.Add(x.Key);
            if (data?.UnwantedAllergens != null)
                foreach (var x in data.UnwantedAllergens.ToArray()) unwantedallergens.Add(x.Key);
            if (data?.Kiosks != null)
                foreach (var x in data.Kiosks.ToArray()) kiosks.Add(x.Key);

            data.CodeTrans = data.Language;

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandTimeout = 500;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_SEARCH_RECIPE]";
            cmd.Parameters.Add("@listview", SqlDbType.Int).Value = data.ListView;
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = data.Type;
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = data.CodeSetPrice;
            cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = data.CodeSet;
            cmd.Parameters.Add("@intNameFilter", SqlDbType.Int).Value = data.NameFilter;
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.Name ?? string.Empty;
            cmd.Parameters.Add("@intNumberFilter", SqlDbType.Int).Value = data.NumberFilter;
            cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 2000).Value = data.Number ?? string.Empty;
            cmd.Parameters.Add("@intPrimaryBrandsFilter", SqlDbType.Int).Value = data.PrimaryBrandsFilter;
            if (primarybrands.Count > 0) cmd.Parameters.Add("@nvcPrimaryBrands", SqlDbType.NVarChar).Value = string.Join(",", primarybrands.ToArray());
            cmd.Parameters.Add("@intUnwantedPrimaryBrandsFilter", SqlDbType.Int).Value = data.UnwantedPrimaryBrandsFilter;
            if (unwantedprimarybrands.Count > 0) cmd.Parameters.Add("@nvcUnwantedPrimaryBrands", SqlDbType.NVarChar).Value = string.Join(",", unwantedprimarybrands.ToArray());
            cmd.Parameters.Add("@intSecondaryBrandsFilter", SqlDbType.Int).Value = data.SecondaryBrandsFilter;
            if (secondarybrands.Count > 0) cmd.Parameters.Add("@nvcSecondaryBrands", SqlDbType.NVarChar).Value = string.Join(",", secondarybrands.ToArray());
            cmd.Parameters.Add("@intUnwantedSecondaryBrandsFilter", SqlDbType.Int).Value = data.UnwantedSecondaryBrandsFilter;
            if (unwantedsecondarybrands.Count > 0) cmd.Parameters.Add("@nvcUnwantedSecondaryBrands", SqlDbType.NVarChar).Value = string.Join(",", unwantedsecondarybrands.ToArray());
            cmd.Parameters.Add("@intKeywordsFilter", SqlDbType.Int).Value = data.KeywordsFilter;
            if (keywords.Count > 0) cmd.Parameters.Add("@nvcKeywords", SqlDbType.NVarChar).Value = string.Join(",", keywords.ToArray());
            cmd.Parameters.Add("@intUnwantedKeywordsFilter", SqlDbType.Int).Value = data.UnwantedKeywordsFilter;
            if (unwantedkeywords.Count > 0) cmd.Parameters.Add("@nvcUnwantedKeywords", SqlDbType.NVarChar).Value = string.Join(",", unwantedkeywords.ToArray());
            cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = data.Category;
            cmd.Parameters.Add("@intRecipeStatus", SqlDbType.Int).Value = data.RecipeStatus;
            cmd.Parameters.Add("@intImage", SqlDbType.Int).Value = data.Image;
            cmd.Parameters.Add("@intAllergensFilter", SqlDbType.Int).Value = data.AllergensFilter;
            if (allergens.Count > 0) cmd.Parameters.Add("@nvcAllergens", SqlDbType.NVarChar).Value = string.Join(",", allergens.ToArray());
            cmd.Parameters.Add("@intUnwantedAllergensFilter", SqlDbType.Int).Value = data.UnwantedAllergensFilter;
            if (unwantedallergens.Count > 0) cmd.Parameters.Add("@nvcUnwantedAllergens", SqlDbType.NVarChar).Value = string.Join(",", unwantedallergens.ToArray());
            cmd.Parameters.Add("@bitWithoutAllergens", SqlDbType.Bit).Value = data.WithoutAllergens;
            cmd.Parameters.Add("@bitWithAtLeastOne", SqlDbType.Bit).Value = data.WithAtLeastOneAllergen;
            cmd.Parameters.Add("@intLanguage", SqlDbType.Int).Value = data.Language;
            cmd.Parameters.Add("@bitVerified", SqlDbType.Bit).Value = data.Verified;
            cmd.Parameters.Add("@intSource", SqlDbType.Int).Value = data.Source;
            cmd.Parameters.Add("@intFilter", SqlDbType.Int).Value = data.SelFilter;
            cmd.Parameters.Add("@intMarkItem", SqlDbType.Int).Value = data.MarkedItems;
            cmd.Parameters.Add("@intUsedAsIngredient", SqlDbType.Int).Value = data.UsedAsIngredient;
            cmd.Parameters.Add("@intWantedIngredientsFilter", SqlDbType.NVarChar).Value = data.WantedMerchandiseFilter;
            cmd.Parameters.Add("@nvcWantedIngredients", SqlDbType.NVarChar).Value = data.WantedMerchandise ?? string.Empty;
            cmd.Parameters.Add("@intUnwantedIngredientsFilter", SqlDbType.NVarChar).Value = data.UnwantedMerchandiseFilter;
            cmd.Parameters.Add("@nvcUnwantedIngredients", SqlDbType.NVarChar).Value = data.UnwantedMerchandise ?? string.Empty;
            cmd.Parameters.Add("@intPriceType", SqlDbType.Int).Value = data.PriceFilter;
            cmd.Parameters.Add("@intPriceOption", SqlDbType.Int).Value = data.PriceOption;
            if (!string.IsNullOrEmpty(data.Price1)) cmd.Parameters.Add("@fltPrice1", SqlDbType.Float).Value = data.Price1;
            if (!string.IsNullOrEmpty(data.Price2)) cmd.Parameters.Add("@fltPrice2", SqlDbType.Float).Value = data.Price2;
            cmd.Parameters.Add("@intDateOption", SqlDbType.Int).Value = data.DateFilter;
            cmd.Parameters.Add("@intPublication", SqlDbType.Int).Value = data.Publication;
            cmd.Parameters.Add("@intPublicationDateOption", SqlDbType.Int).Value = data.PublicationDateFilter;
            cmd.Parameters.Add("@intKioskFilter", SqlDbType.Int).Value = data.KioskFilter;
            cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.Skip;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.Take;
            if (kiosks.Count > 0) cmd.Parameters.Add("@nvcKiosks", SqlDbType.NVarChar).Value = string.Join(",", kiosks.ToArray());
            if (data.DateFrom != DateTime.MinValue) cmd.Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = GetDateOnly(data.DateFrom);
            if (data.DateTo != DateTime.MinValue) cmd.Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = GetDateOnly(data.DateTo);
            if (data.PublicationDateFrom != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate1", SqlDbType.DateTime).Value = GetDateOnly(data.PublicationDateFrom);
            if (data.PublicationDateTo != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate2", SqlDbType.DateTime).Value = GetDateOnly(data.PublicationDateTo);
            cmd.Parameters.Add("@FullText", SqlDbType.Bit).Value = data.FullText;
            cmd.Parameters.Add("@InitialLoad", SqlDbType.Int).Value = data.InitialLoad;
            cmd.Parameters.Add("@intTime", SqlDbType.Int).Value = data.Time;
            cmd.Parameters.Add("@intTimesFilter", SqlDbType.Int).Value = data.TimesFilter;
            cmd.Parameters.Add("@intDatesFilter", SqlDbType.Int).Value = data.DatesFilter;
            cmd.Parameters.Add("@bitNotTranslated", SqlDbType.Bit).Value = data.NotTranslated;

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read()) defformat.Add(GetStr(dr["format"]).Trim());
                while (defformat.Count < 34) defformat.Add("#,##0.0");
            }
            dr.NextResult();
            if (dr.HasRows) { while (dr.Read()) totalCount = GetInt(dr["Total"]); }

            dr.NextResult();
            if (!dr.HasRows) return Ok(false);

            var listViewValue = data.ListView;

            if (listViewValue == 2)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        PrimaryBrand = GetStr(dr["PrimaryBrand"]),
                        SecondaryBrand = GetStr(dr["SecondaryBrand"]),
                        RecipeStatus = GetStr(dr["RecipeStatus"]),
                        Image = GetBool(dr["ImageDisplay"]),
                        PictureName = GetStr(dr["PictureName"]),
                        Nutrition = GetBool(dr["DisplayNutrition"]),
                        DateCreated = GetStr(dr["Dates"]),
                        Category = GetStr(dr["Category"]),
                        Owner = GetStr(dr["Owner"]),
                        Yield = GetDbl(dr["Yield"]),
                        YieldFormat = GetStr(dr["YieldFormat"]),
                        PriceFormat = GetStr(dr["PriceFormat"]),
                        YieldName = GetStr(dr["YieldName"]),
                        Source = GetStr(dr["Source"]),
                        Unit = GetStr(dr["YieldName"]),
                        Status = GetStr(dr["Status"]),
                        CalcPrice = GetDbl(dr["CalcPrice"]),
                        ImposedPrice = GetDbl(dr["ImposedPrice"]),
                        Currency = GetStr(dr["Currency"]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        FoodCost = GetDbl(dr["FoodCost"]),
                        FoodCostPercent = GetDbl(dr["FoodCostPercent"]),
                        GrossMargin = GetDbl(dr["GrossMargin"]),
                        GrossMarginPercent = GetDbl(dr["GrossMarginPercent"]),
                        NetMargin = GetDbl(dr["NetMargin"]),
                        NetMarginPercent = GetDbl(dr["NetMarginPercent"]),
                        ImposedSellingPriceWOTax = GetDbl(dr["ImposedSellingPriceWOTax"]),
                        ImposedSellingPriceWTax = GetDbl(dr["ImposedSellingPriceWTax"]),
                        PimFlag = GetInt(dr["PIMFlag"]),
                        WithTranslation = GetInt(dr["WithTranslation"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (listViewValue == 1)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        PictureName = GetStr(dr["PictureName"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (listViewValue == 5)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        Contains = GetStr(dr["AllergenContain"]),
                        NonAllergens = GetStr(dr["AllergenNonAllergen"]),
                        CompleteAllergen = GetStr(dr["CompleteAllergen"]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (listViewValue == 4)
            {
                var lists = new List<NutrientList>();
                while (dr.Read())
                {
                    lists.Add(new NutrientList
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        N1_1 = FormatValue(dr["N1_1"], defformat[0]),
                        N1_2 = FormatValue(dr["N1_2"], defformat[0]),
                        N2 = FormatValue(dr["N2"], defformat[1]),
                        N3 = FormatValue(dr["N3"], defformat[2]),
                        N4 = FormatValue(dr["N4"], defformat[3]),
                        N5 = FormatValue(dr["N5"], defformat[4]),
                        N6 = FormatValue(dr["N6"], defformat[5]),
                        N7 = FormatValue(dr["N7"], defformat[6]),
                        N8 = FormatValue(dr["N8"], defformat[7]),
                        N9 = FormatValue(dr["N9"], defformat[8]),
                        N10 = FormatValue(dr["N10"], defformat[9]),
                        N11 = FormatValue(dr["N11"], defformat[10]),
                        N12 = FormatValue(dr["N12"], defformat[11]),
                        N13 = FormatValue(dr["N13"], defformat[12]),
                        N14 = FormatValue(dr["N14"], defformat[13]),
                        N15 = FormatValue(dr["N15"], defformat[14]),
                        N16 = FormatValue(dr["N16"], defformat[15]),
                        N17 = FormatValue(dr["N17"], defformat[16]),
                        N18 = FormatValue(dr["N18"], defformat[17]),
                        N19 = FormatValue(dr["N19"], defformat[18]),
                        N20 = FormatValue(dr["N20"], defformat[19]),
                        N21 = FormatValue(dr["N21"], defformat[20]),
                        N22 = FormatValue(dr["N22"], defformat[21]),
                        N23 = FormatValue(dr["N23"], defformat[22]),
                        N24 = FormatValue(dr["N24"], defformat[23]),
                        N25 = FormatValue(dr["N25"], defformat[24]),
                        N26 = FormatValue(dr["N26"], defformat[25]),
                        N27 = FormatValue(dr["N27"], defformat[26]),
                        N28 = FormatValue(dr["N28"], defformat[27]),
                        N29 = FormatValue(dr["N29"], defformat[28]),
                        N30 = FormatValue(dr["N30"], defformat[29]),
                        N31 = FormatValue(dr["N31"], defformat[30]),
                        N32 = FormatValue(dr["N32"], defformat[31]),
                        N33 = FormatValue(dr["N33"], defformat[32]),
                        N34 = FormatValue(dr["N34"], defformat[33]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericNutrients(lists, totalCount));
            }

            if (listViewValue == 6)
            {
                var dt = new DataTable("Customers");
                dt.Load(dr);
                return Ok(ToReturn(dt, totalCount));
            }

            return Ok(false);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchRecipeData failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("api/search/merchandise")]
    public ActionResult<object> SearchMerchandise([FromBody] Searchee data)
    {
        try
        {
            int totalCount = 0;
            var price1 = ParseLocalizedDouble(data.Price1, data.CodeTrans);
            var price2 = ParseLocalizedDouble(data.Price2, data.CodeTrans);

            var brands = new ArrayList();
            var keywords = new ArrayList();
            var unwantedkeywords = new ArrayList();
            var allergens = new ArrayList();
            var unwantedallergens = new ArrayList();
            var defformat = new List<string>();

            if (data?.Brands != null) foreach (var x in data.Brands.ToArray()) brands.Add(x.Key);
            if (data?.Keywords != null) foreach (var x in data.Keywords.ToArray()) keywords.Add(x.Key);
            if (data?.UnwantedKeywords != null) foreach (var x in data.UnwantedKeywords.ToArray()) unwantedkeywords.Add(x.Key);
            if (data?.Allergens != null) foreach (var x in data.Allergens.ToArray()) allergens.Add(x.Key);
            if (data?.UnwantedAllergens != null) foreach (var x in data.UnwantedAllergens.ToArray()) unwantedallergens.Add(x.Key);

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandTimeout = 500;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_SEARCH_MERCHANDISE]";
            cmd.Parameters.Add("@listview", SqlDbType.Int).Value = data.ListView;
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = data.Type;
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.Language;
            cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = data.CodeSetPrice;
            cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = data.CodeSet;
            cmd.Parameters.Add("@intNameFilter", SqlDbType.Int).Value = data.NameFilter;
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.Name ?? string.Empty;
            cmd.Parameters.Add("@intNumberFilter", SqlDbType.Int).Value = data.NumberFilter;
            cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 2000).Value = data.Number ?? string.Empty;
            cmd.Parameters.Add("@intBrandsFilter", SqlDbType.Int).Value = 0;
            if (brands.Count > 0) cmd.Parameters.Add("@nvcBrands", SqlDbType.NVarChar).Value = string.Join(",", brands.ToArray());
            cmd.Parameters.Add("@intSupplier", SqlDbType.Int).Value = data.Supplier;
            cmd.Parameters.Add("@intKeywordsFilter", SqlDbType.Int).Value = data.KeywordsFilter;
            if (keywords.Count > 0) cmd.Parameters.Add("@nvcKeywords", SqlDbType.NVarChar).Value = string.Join(",", keywords.ToArray());
            cmd.Parameters.Add("@intUnwantedKeywordsFilter", SqlDbType.Int).Value = data.UnwantedKeywordsFilter;
            if (unwantedkeywords.Count > 0) cmd.Parameters.Add("@nvcUnwantedKeywords", SqlDbType.NVarChar).Value = string.Join(",", unwantedkeywords.ToArray());
            cmd.Parameters.Add("@intAllergensFilter", SqlDbType.Int).Value = data.AllergensFilter;
            if (allergens.Count > 0) cmd.Parameters.Add("@nvcAllergens", SqlDbType.NVarChar).Value = string.Join(",", allergens.ToArray());
            cmd.Parameters.Add("@intUnwantedAllergensFilter", SqlDbType.Int).Value = data.UnwantedAllergensFilter;
            if (unwantedallergens.Count > 0) cmd.Parameters.Add("@nvcUnwantedAllergens", SqlDbType.NVarChar).Value = string.Join(",", unwantedallergens.ToArray());
            cmd.Parameters.Add("@bitWithoutAllergens", SqlDbType.Bit).Value = data.WithoutAllergens;
            cmd.Parameters.Add("@bitWithAtLeastOne", SqlDbType.Bit).Value = data.WithAtLeastOneAllergen;
            cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = data.Category;
            cmd.Parameters.Add("@intLanguage", SqlDbType.Int).Value = data.Language;
            cmd.Parameters.Add("@intFilter", SqlDbType.Int).Value = data.SelFilter;
            cmd.Parameters.Add("@intMarkItem", SqlDbType.Int).Value = data.MarkedItems;
            cmd.Parameters.Add("@intUsedAsIngredient", SqlDbType.Int).Value = data.UsedAsIngredient;
            cmd.Parameters.Add("@intPriceType", SqlDbType.Int).Value = data.PriceFilter;
            cmd.Parameters.Add("@intPriceOption", SqlDbType.Int).Value = data.PriceOption;
            cmd.Parameters.Add("@fltPrice1", SqlDbType.Float).Value = string.IsNullOrEmpty(data.Price1) ? 0 : price1;
            cmd.Parameters.Add("@fltPrice2", SqlDbType.Float).Value = string.IsNullOrEmpty(data.Price2) ? 0 : price2;
            cmd.Parameters.Add("@intDateOption", SqlDbType.Int).Value = data.DateFilter;
            cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.Skip;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.Take;
            if (data.DateFrom != DateTime.MinValue) cmd.Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = GetDateOnly(data.DateFrom);
            if (data.DateTo != DateTime.MinValue) cmd.Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = GetDateOnly(data.DateTo);
            cmd.Parameters.Add("@FullText", SqlDbType.Bit).Value = data.FullText;
            cmd.Parameters.Add("@intStatus", SqlDbType.Int).Value = data.Status;
            cmd.Parameters.Add("@InitialLoad", SqlDbType.Int).Value = data.InitialLoad;
            cmd.Parameters.Add("@intIssue", SqlDbType.Int).Value = data.Issue;
            cmd.Parameters.Add("@bitNotTranslated", SqlDbType.Bit).Value = data.NotTranslated;

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read()) defformat.Add(GetStr(dr["format"]));
                while (defformat.Count < 34) defformat.Add("#,##0.0");
            }
            dr.NextResult();
            if (dr.HasRows) { while (dr.Read()) totalCount = GetInt(dr["Total"]); }
            dr.NextResult();

            if (!dr.HasRows) return Ok(false);

            if (data.ListView == 2 || data.ListView == 1)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        Nutrition = DisplayNutr(GetInt(dr["Code"]), data.CodeSet),
                        Category = GetStr(dr["Category"]),
                        Brand = GetStr(dr["Brand"]),
                        Supplier = GetStr(dr["Supplier"]),
                        Tax = GetDbl(dr["Tax"]),
                        Owner = GetStr(dr["Owner"]),
                        Status = GetStr(dr["Status"]),
                        Issue = GetStr(dr["PIMIssue"]),
                        DateCreated = GetStr(dr["Dates"]),
                        Image = GetBool(dr["ImageDisplay"]),
                        PictureName = GetStr(dr["PictureName"]),
                        Unit = GetStr(dr["Unit"]),
                        Currency = GetStr(dr["Currency"]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        WithTranslation = GetInt(dr["WithTranslation"]),
                        PimStatus = GetInt(dr["PIMStatus"]),
                        IsLocked = GetBool(dr["IsLocked"]),
                        PriceFormat = GetStr(dr["priceFormat"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (data.ListView == 5)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        Contains = GetStr(dr["AllergenContain"]),
                        NonAllergens = GetStr(dr["AllergenNonAllergen"]),
                        CompleteAllergen = GetStr(dr["CompleteAllergen"]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (data.ListView == 4)
            {
                var lists = new List<NutrientList>();
                while (dr.Read())
                {
                    lists.Add(new NutrientList
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        N1_1 = FormatValue(dr["N1_1"], "#,##0.0"),
                        N1_2 = FormatValue(dr["N1_2"], "#,##0.0"),
                        N2 = FormatValue(dr["N2"], "#,##0.0"),
                        N3 = FormatValue(dr["N3"], "#,##0.0"),
                        N4 = FormatValue(dr["N4"], "#,##0.0"),
                        N5 = FormatValue(dr["N5"], "#,##0.0"),
                        N6 = FormatValue(dr["N6"], "#,##0.0"),
                        N7 = FormatValue(dr["N7"], "#,##0.0"),
                        N8 = FormatValue(dr["N8"], "#,##0.0"),
                        N9 = FormatValue(dr["N9"], "#,##0.0"),
                        N10 = FormatValue(dr["N10"], "#,##0.0"),
                        N11 = FormatValue(dr["N11"], "#,##0.0"),
                        N12 = FormatValue(dr["N12"], "#,##0.0"),
                        N13 = FormatValue(dr["N13"], "#,##0.0"),
                        N14 = FormatValue(dr["N14"], "#,##0.0"),
                        N15 = FormatValue(dr["N15"], "#,##0.0"),
                        N16 = FormatValue(dr["N16"], "#,##0.0"),
                        N17 = FormatValue(dr["N17"], "#,##0.0"),
                        N18 = FormatValue(dr["N18"], "#,##0.0"),
                        N19 = FormatValue(dr["N19"], "#,##0.0"),
                        N20 = FormatValue(dr["N20"], "#,##0.0"),
                        N21 = FormatValue(dr["N21"], "#,##0.0"),
                        N22 = FormatValue(dr["N22"], "#,##0.0"),
                        N23 = FormatValue(dr["N23"], "#,##0.0"),
                        N24 = FormatValue(dr["N24"], "#,##0.0"),
                        N25 = FormatValue(dr["N25"], "#,##0.0"),
                        N26 = FormatValue(dr["N26"], "#,##0.0"),
                        N27 = FormatValue(dr["N27"], "#,##0.0"),
                        N28 = FormatValue(dr["N28"], "#,##0.0"),
                        N29 = FormatValue(dr["N29"], "#,##0.0"),
                        N30 = FormatValue(dr["N30"], "#,##0.0"),
                        N31 = FormatValue(dr["N31"], "#,##0.0"),
                        N32 = FormatValue(dr["N32"], "#,##0.0"),
                        N33 = FormatValue(dr["N33"], "#,##0.0"),
                        N34 = FormatValue(dr["N34"], "#,##0.0"),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericNutrients(lists, totalCount));
            }

            return Ok(false);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchMerchandise failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("api/search/menu")]
    public ActionResult<object> SearchMenuData([FromBody] Searchee data)
    {
        try
        {
            int totalCount = 0;

            var keywords = new ArrayList();
            var unwantedkeywords = new ArrayList();
            var allergens = new ArrayList();
            var unwantedallergens = new ArrayList();

            if (data?.Keywords != null) foreach (var x in data.Keywords.ToArray()) keywords.Add(x.Key);
            if (data?.UnwantedKeywords != null) foreach (var x in data.UnwantedKeywords.ToArray()) unwantedkeywords.Add(x.Key);
            if (data?.Allergens != null) foreach (var x in data.Allergens.ToArray()) allergens.Add(x.Key);
            if (data?.UnwantedAllergens != null) foreach (var x in data.UnwantedAllergens.ToArray()) unwantedallergens.Add(x.Key);

            if (!double.TryParse(data.Price1, out _)) data.Price1 = "0";
            if (!double.TryParse(data.Price2, out _)) data.Price2 = "0";
            if (data.Language != 0) data.CodeTrans = data.Language;

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_SEARCH_MENU]";
            cmd.Parameters.Add("@listview", SqlDbType.Int).Value = data.ListView;
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = data.Type;
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = data.CodeSetPrice;
            cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = data.CodeSet;
            cmd.Parameters.Add("@intNameFilter", SqlDbType.Int).Value = data.NameFilter;
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.Name ?? string.Empty;
            cmd.Parameters.Add("@intNumberFilter", SqlDbType.Int).Value = data.NumberFilter;
            cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 2000).Value = data.Number ?? string.Empty;
            cmd.Parameters.Add("@intPrimaryBrandsFilter", SqlDbType.Int).Value = data.PrimaryBrandsFilter;
            cmd.Parameters.Add("@intKeywordsFilter", SqlDbType.Int).Value = data.KeywordsFilter;
            if (keywords.Count > 0) cmd.Parameters.Add("@nvcKeywords", SqlDbType.NVarChar).Value = string.Join(",", keywords.ToArray());
            cmd.Parameters.Add("@intUnwantedKeywordsFilter", SqlDbType.Int).Value = data.UnwantedKeywordsFilter;
            if (unwantedkeywords.Count > 0) cmd.Parameters.Add("@nvcUnwantedKeywords", SqlDbType.NVarChar).Value = string.Join(",", unwantedkeywords.ToArray());
            cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = data.Category;
            cmd.Parameters.Add("@intRecipeStatus", SqlDbType.Int).Value = data.RecipeStatus;
            cmd.Parameters.Add("@intImage", SqlDbType.Int).Value = data.Image;
            cmd.Parameters.Add("@intAllergensFilter", SqlDbType.Int).Value = data.AllergensFilter;
            if (allergens.Count > 0) cmd.Parameters.Add("@nvcAllergens", SqlDbType.NVarChar).Value = string.Join(",", allergens.ToArray());
            cmd.Parameters.Add("@intUnwantedAllergensFilter", SqlDbType.Int).Value = data.UnwantedAllergensFilter;
            if (unwantedallergens.Count > 0) cmd.Parameters.Add("@nvcUnwantedAllergens", SqlDbType.NVarChar).Value = string.Join(",", unwantedallergens.ToArray());
            cmd.Parameters.Add("@bitWithoutAllergens", SqlDbType.Bit).Value = data.WithoutAllergens;
            cmd.Parameters.Add("@bitWithAtLeastOne", SqlDbType.Bit).Value = data.WithAtLeastOneAllergen;
            cmd.Parameters.Add("@intLanguage", SqlDbType.Int).Value = data.Language;
            cmd.Parameters.Add("@bitVerified", SqlDbType.Bit).Value = data.Verified;
            cmd.Parameters.Add("@intSource", SqlDbType.Int).Value = data.Source;
            cmd.Parameters.Add("@intFilter", SqlDbType.Int).Value = data.SelFilter;
            cmd.Parameters.Add("@intMarkItem", SqlDbType.Int).Value = data.MarkedItems;
            cmd.Parameters.Add("@intUsedAsIngredient", SqlDbType.Int).Value = data.UsedAsIngredient;
            cmd.Parameters.Add("@intWantedIngredientsFilter", SqlDbType.NVarChar).Value = data.WantedMerchandiseFilter;
            cmd.Parameters.Add("@nvcWantedIngredients", SqlDbType.NVarChar).Value = data.WantedMerchandise ?? string.Empty;
            cmd.Parameters.Add("@intUnwantedIngredientsFilter", SqlDbType.NVarChar).Value = data.UnwantedMerchandiseFilter;
            cmd.Parameters.Add("@nvcUnwantedIngredients", SqlDbType.NVarChar).Value = data.UnwantedMerchandise ?? string.Empty;
            cmd.Parameters.Add("@intPriceType", SqlDbType.Int).Value = data.PriceFilter;
            cmd.Parameters.Add("@intPriceOption", SqlDbType.Int).Value = data.PriceOption;
            cmd.Parameters.Add("@fltPrice1", SqlDbType.Float).Value = Convert.ToDouble(data.Price1, CultureInfo.InvariantCulture);
            cmd.Parameters.Add("@fltPrice2", SqlDbType.Float).Value = Convert.ToDouble(data.Price2, CultureInfo.InvariantCulture);
            cmd.Parameters.Add("@intDateOption", SqlDbType.Int).Value = data.DateFilter;
            cmd.Parameters.Add("@intPublication", SqlDbType.Int).Value = data.Publication;
            cmd.Parameters.Add("@intPublicationDateOption", SqlDbType.Int).Value = data.PublicationDateFilter;
            cmd.Parameters.Add("@intKioskFilter", SqlDbType.Int).Value = data.KioskFilter;
            cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.Skip;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.Take;
            if (data.DateFrom != DateTime.MinValue) cmd.Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = GetDateOnly(data.DateFrom);
            if (data.DateTo != DateTime.MinValue) cmd.Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = GetDateOnly(data.DateTo);
            if (data.PublicationDateFrom != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate1", SqlDbType.DateTime).Value = data.PublicationDateFrom;
            if (data.PublicationDateTo != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate2", SqlDbType.DateTime).Value = data.PublicationDateTo;
            cmd.Parameters.Add("@FullText", SqlDbType.Bit).Value = data.FullText;
            cmd.Parameters.Add("@InitialLoad", SqlDbType.Int).Value = data.InitialLoad;
            cmd.Parameters.Add("@bitNotTranslated", SqlDbType.Bit).Value = data.NotTranslated;

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read()) totalCount = GetInt(dr["Total"]);
            }
            dr.NextResult();
            if (!dr.HasRows) return Ok(false);

            if (data.ListView == 2)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        PrimaryBrand = GetStr(dr["PrimaryBrand"]),
                        SecondaryBrand = GetStr(dr["SecondaryBrand"]),
                        RecipeStatus = GetStr(dr["RecipeStatus"]),
                        Image = GetBool(dr["ImageDisplay"]),
                        PictureName = GetStr(dr["PictureName"]),
                        Nutrition = GetBool(dr["DisplayNutrition"]),
                        DateCreated = GetStr(dr["Dates"]),
                        Category = GetStr(dr["Category"]),
                        Owner = GetStr(dr["Owner"]),
                        Yield = GetDbl(dr["Yield"]),
                        YieldFormat = GetStr(dr["YieldFormat"]),
                        PriceFormat = GetStr(dr["PriceFormat"]),
                        YieldName = GetStr(dr["YieldName"]),
                        Source = GetStr(dr["Source"]),
                        Unit = GetStr(dr["Unit"]),
                        Status = GetStr(dr["Status"]),
                        CalcPrice = GetDbl(dr["CalcPrice"]),
                        ImposedPrice = GetDbl(dr["ImposedPrice"]),
                        Currency = GetStr(dr["Currency"]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (data.ListView == 1)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        PictureName = GetStr(dr["PictureName"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (data.ListView == 5)
            {
                var lists = new List<GenericSearch>();
                while (dr.Read())
                {
                    lists.Add(new GenericSearch
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        Contains = GetStr(dr["AllergenContain"]),
                        NonAllergens = GetStr(dr["AllergenNonAllergen"]),
                        CompleteAllergen = GetStr(dr["CompleteAllergen"]),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericSearch(lists, totalCount));
            }

            if (data.ListView == 4)
            {
                var lists = new List<NutrientList>();
                while (dr.Read())
                {
                    lists.Add(new NutrientList
                    {
                        Code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        Number = GetStr(dr["Number"]),
                        N1_1 = FormatValue(dr["N1_1"], "#,##0.0"),
                        N1_2 = FormatValue(dr["N1_2"], "#,##0.0"),
                        N2 = FormatValue(dr["N2"], "#,##0.0"),
                        N3 = FormatValue(dr["N3"], "#,##0.0"),
                        N4 = FormatValue(dr["N4"], "#,##0.0"),
                        N5 = FormatValue(dr["N5"], "#,##0.0"),
                        N6 = FormatValue(dr["N6"], "#,##0.0"),
                        N7 = FormatValue(dr["N7"], "#,##0.0"),
                        N8 = FormatValue(dr["N8"], "#,##0.0"),
                        N9 = FormatValue(dr["N9"], "#,##0.0"),
                        N10 = FormatValue(dr["N10"], "#,##0.0"),
                        N11 = FormatValue(dr["N11"], "#,##0.0"),
                        N12 = FormatValue(dr["N12"], "#,##0.0"),
                        N13 = FormatValue(dr["N13"], "#,##0.0"),
                        N14 = FormatValue(dr["N14"], "#,##0.0"),
                        N15 = FormatValue(dr["N15"], "#,##0.0"),
                        N16 = FormatValue(dr["N16"], "#,##0.0"),
                        N17 = FormatValue(dr["N17"], "#,##0.0"),
                        N18 = FormatValue(dr["N18"], "#,##0.0"),
                        N19 = FormatValue(dr["N19"], "#,##0.0"),
                        N20 = FormatValue(dr["N20"], "#,##0.0"),
                        N21 = FormatValue(dr["N21"], "#,##0.0"),
                        N22 = FormatValue(dr["N22"], "#,##0.0"),
                        N23 = FormatValue(dr["N23"], "#,##0.0"),
                        N24 = FormatValue(dr["N24"], "#,##0.0"),
                        N25 = FormatValue(dr["N25"], "#,##0.0"),
                        N26 = FormatValue(dr["N26"], "#,##0.0"),
                        N27 = FormatValue(dr["N27"], "#,##0.0"),
                        N28 = FormatValue(dr["N28"], "#,##0.0"),
                        N29 = FormatValue(dr["N29"], "#,##0.0"),
                        N30 = FormatValue(dr["N30"], "#,##0.0"),
                        N31 = FormatValue(dr["N31"], "#,##0.0"),
                        N32 = FormatValue(dr["N32"], "#,##0.0"),
                        N33 = FormatValue(dr["N33"], "#,##0.0"),
                        N34 = FormatValue(dr["N34"], "#,##0.0"),
                        CheckoutUser = GetInt(dr["Checkoutuser"]),
                        IsLocked = GetBool(dr["IsLocked"])
                    });
                }
                return Ok(new ResponseGenericNutrients(lists, totalCount));
            }

            return Ok(false);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchMenuData failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("api/search/menuplan")]
    public ActionResult<object> SearchMenuPlanData([FromBody] Searchee data)
    {
        try
        {
            int totalCount = 0;
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_SEARCH_MENUPLAN]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@NameFilter", SqlDbType.Int).Value = data.NameFilter;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 2000).Value = data.Name ?? string.Empty;
            cmd.Parameters.Add("@NumberFilter", SqlDbType.Int).Value = data.NumberFilter;
            cmd.Parameters.Add("@Number", SqlDbType.NVarChar, 2000).Value = data.Number ?? string.Empty;
            cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = data.Category;
            cmd.Parameters.Add("@CodeSeason", SqlDbType.Int).Value = data.Season;
            cmd.Parameters.Add("@CodeService", SqlDbType.Int).Value = data.ServiceType;
            cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.Skip;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.Take;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows) { while (dr.Read()) totalCount = GetInt(dr["Total"]); }
            dr.NextResult();
            if (!dr.HasRows) return Ok(false);

            var lists = new List<MenuplanSearch>();
            while (dr.Read())
            {
                lists.Add(new MenuplanSearch
                {
                    Code = GetInt(dr["Code"]),
                    Name = GetStr(dr["Name"]),
                    Number = GetStr(dr["Number"]),
                    Restaurant = GetStr(dr["Restaurant"]),
                    CodeRestaurant = GetStr(dr["CodeRestaurant"]),
                    CyclePlan = GetBool(dr["CyclePlan"]),
                    StartDate = GetDate(dr["StartDate"]).ToString(CultureInfo.InvariantCulture),
                    Duration = GetInt(dr["Duration"]),
                    Recurrence = GetInt(dr["Recurrence"]),
                    Category = GetStr(dr["Category"]),
                    Season = GetStr(dr["Season"]),
                    ServiceType = GetStr(dr["ServiceType"])
                });
            }
            return Ok(new ResponseMenuPlanSearch(lists, totalCount));
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchMenuPlanData failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private static List<TreeNode> CreateChildren(List<GenericTree> recipe, int code)
    {
        var children = new List<TreeNode>();
        if (recipe != null)
        {
            var kids = recipe.Where(o => o.Code != code && o.ParentCode == code && code > 0).OrderBy(o => o.Name).ToList();
            foreach (var k in kids)
            {
                var child = new TreeNode
                {
                    Title = k.Name,
                    Key = k.Code,
                    Icon = false,
                    Children = CreateChildren(recipe, k.Code),
                    Selected = k.Flagged,
                    ParentTitle = k.ParentName,
                    Note = k.Note,
                    Link = k.Link
                };
                children.Add(child);
            }
        }
        return children;
    }

    private static string GetDateOnly(DateTime dt)
    {
        return dt.ToShortDateString();
    }

    private static string FormatValue(object value, string fmt)
    {
        if (value == null || value == DBNull.Value) return string.Empty;
        if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            return d.ToString(fmt, CultureInfo.InvariantCulture);
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static double ParseLocalizedDouble(string? num, int codetrans)
    {
        if (string.IsNullOrWhiteSpace(num)) return 0;
        var dec = codetrans == 3 ? ',' : '.';
        var thou = codetrans == 3 ? '.' : ',';
        var parts = num.Split(dec);
        parts[0] = parts[0].Replace(thou.ToString(), string.Empty);
        var joined = string.Join(".", parts);
        if (double.TryParse(joined, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
        return 0;
    }

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
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
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

    private static object ToReturn(DataTable dt, int totalCount)
    {
        return new { data = dt, totalCount };
    }

    private static bool DisplayNutr(int codeListe, int codeNutrientSet)
    {
        return false;
    }
}
