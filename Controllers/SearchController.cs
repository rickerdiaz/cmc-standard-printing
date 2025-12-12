using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

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
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            primarybrand = GetStr(dr["PrimaryBrand"]),
                            secondarybrand = GetStr(dr["SecondaryBrand"]),
                            recipeStatus = GetStr(dr["RecipeStatus"]),
                            image = GetBool(dr["ImageDisplay"]),
                            picturename = GetStr(dr["PictureName"]),
                            nutrition = GetBool(dr["DisplayNutrition"]),
                            dateCreated = GetDate(dr["Dates"]),
                            category = GetStr(dr["Category"]),
                            owner = GetStr(dr["Owner"]),
                            yield = GetInt(dr["Yield"]),
                            yieldFormat = GetStr(dr["YieldFormat"]),
                            yieldName = GetStr(dr["YieldName"]),
                            source = GetStr(dr["Source"]),
                            unit = GetStr(dr["Yield"]),
                            status = GetStr(dr["Status"]),
                            calcPrice = GetDbl(dr["CalcPrice"]),
                            imposedPrice = GetDbl(dr["ImposedPrice"]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            FoodCost = GetDbl(dr["FoodCost"]),
                            FoodCostPercent = GetDbl(dr["FoodCostPercent"]),
                            GrossMargin = GetDbl(dr["GrossMargin"]),
                            GrossMarginPercent = GetDbl(dr["GrossMarginPercent"]),
                            NetMargin = GetDbl(dr["NetMargin"]),
                            NetMarginPercent = GetDbl(dr["NetMarginPercent"]),
                            ImposedSellingPriceWOTax = GetDbl(dr["ImposedSellingPriceWOTax"]),
                            ImposedSellingPriceWTax = GetDbl(dr["ImposedSellingPriceWTax"]),
                            PIMFlag = GetInt(dr["PIMFlag"]),
                            DateTested = GetDate(dr["DateTested"]),
                            withTranslation = GetInt(dr["WithTranslation"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, lists.Count));
                }
                else
                {
                    var recipes = new List<Models.GenericTree>();
                    while (dr.Read())
                    {
                        recipes.Add(new Models.GenericTree
                        {
                            Code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            Number = GetStr(dr["Number"]),
                            ParentCode = GetInt(dr["ParentCode"]),
                            link = GetStr(dr["Link"]),
                            Flagged = GetBool(dr["Flag"]),
                            Note = GetStr(dr["Note"]) 
                        });
                    }

                    var list = new List<Models.TreeNode>();
                    var parents = recipes.Where(o => o.ParentCode == 0 || o.ParentCode == null).OrderBy(o => o.Name).ToList();
                    foreach (var p in parents)
                    {
                        if (list.All(o => o.key != p.Code))
                        {
                            var parent = new Models.TreeNode
                            {
                                title = (!string.IsNullOrEmpty(p.Number) ? $"[{p.Number}] - " : string.Empty) + p.Name,
                                key = p.Code,
                                icon = false,
                                children = CreateChildren(recipes, p.Code),
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
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/search/recipe")]
        public ActionResult<object> SearchRecipeData([FromBody] Models.Searchee data)
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

                if (data?.primarybrands != null)
                    foreach (var x in data.primarybrands.ToArray()) primarybrands.Add(x.key);
                if (data?.unwantedprimarybrands != null)
                    foreach (var x in data.unwantedprimarybrands.ToArray()) unwantedprimarybrands.Add(x.key);
                if (data?.secondarybrands != null)
                    foreach (var x in data.secondarybrands.ToArray()) secondarybrands.Add(x.key);
                if (data?.unwantedsecondarybrands != null)
                    foreach (var x in data.unwantedsecondarybrands.ToArray()) unwantedsecondarybrands.Add(x.key);
                if (data?.keywords != null)
                    foreach (var x in data.keywords.ToArray()) keywords.Add(x.key);
                if (data?.unwantedkeywords != null)
                    foreach (var x in data.unwantedkeywords.ToArray()) unwantedkeywords.Add(x.key);
                if (data?.allergens != null)
                    foreach (var x in data.allergens.ToArray()) allergens.Add(x.key);
                if (data?.unwantedallergens != null)
                    foreach (var x in data.unwantedallergens.ToArray()) unwantedallergens.Add(x.key);
                if (data?.kiosks != null)
                    foreach (var x in data.kiosks.ToArray()) kiosks.Add(x.key);

                data.codetrans = data.language;

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_SEARCH_RECIPE]";
                cmd.Parameters.Add("@listview", SqlDbType.Int).Value = data.listview;
                cmd.Parameters.Add("@intType", SqlDbType.Int).Value = data.type;
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.codeuser;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.codesite;
                cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.codetrans;
                cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = data.codesetprice;
                cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = data.codeset;
                cmd.Parameters.Add("@intNameFilter", SqlDbType.Int).Value = data.namefilter;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.name ?? string.Empty;
                cmd.Parameters.Add("@intNumberFilter", SqlDbType.Int).Value = data.numberfilter;
                cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 2000).Value = data.number ?? string.Empty;
                cmd.Parameters.Add("@intPrimaryBrandsFilter", SqlDbType.Int).Value = data.primarybrandsfilter;
                if (primarybrands.Count > 0) cmd.Parameters.Add("@nvcPrimaryBrands", SqlDbType.NVarChar).Value = string.Join(",", primarybrands.ToArray());
                cmd.Parameters.Add("@intUnwantedPrimaryBrandsFilter", SqlDbType.Int).Value = data.unwantedprimarybrandsfilter;
                if (unwantedprimarybrands.Count > 0) cmd.Parameters.Add("@nvcUnwantedPrimaryBrands", SqlDbType.NVarChar).Value = string.Join(",", unwantedprimarybrands.ToArray());
                cmd.Parameters.Add("@intSecondaryBrandsFilter", SqlDbType.Int).Value = data.secondarybrandsfilter;
                if (secondarybrands.Count > 0) cmd.Parameters.Add("@nvcSecondaryBrands", SqlDbType.NVarChar).Value = string.Join(",", secondarybrands.ToArray());
                cmd.Parameters.Add("@intUnwantedSecondaryBrandsFilter", SqlDbType.Int).Value = data.unwantedsecondarybrandsfilter;
                if (unwantedsecondarybrands.Count > 0) cmd.Parameters.Add("@nvcUnwantedSecondaryBrands", SqlDbType.NVarChar).Value = string.Join(",", unwantedsecondarybrands.ToArray());
                cmd.Parameters.Add("@intKeywordsFilter", SqlDbType.Int).Value = data.keywordsfilter;
                if (keywords.Count > 0) cmd.Parameters.Add("@nvcKeywords", SqlDbType.NVarChar).Value = string.Join(",", keywords.ToArray());
                cmd.Parameters.Add("@intUnwantedKeywordsFilter", SqlDbType.Int).Value = data.unwantedkeywordsfilter;
                if (unwantedkeywords.Count > 0) cmd.Parameters.Add("@nvcUnwantedKeywords", SqlDbType.NVarChar).Value = string.Join(",", unwantedkeywords.ToArray());
                cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = data.category;
                cmd.Parameters.Add("@intRecipeStatus", SqlDbType.Int).Value = data.recipestatus;
                cmd.Parameters.Add("@intImage", SqlDbType.Int).Value = data.image;
                cmd.Parameters.Add("@intAllergensFilter", SqlDbType.Int).Value = data.allergensfilter;
                if (allergens.Count > 0) cmd.Parameters.Add("@nvcAllergens", SqlDbType.NVarChar).Value = string.Join(",", allergens.ToArray());
                cmd.Parameters.Add("@intUnwantedAllergensFilter", SqlDbType.Int).Value = data.unwantedallergensfilter;
                if (unwantedallergens.Count > 0) cmd.Parameters.Add("@nvcUnwantedAllergens", SqlDbType.NVarChar).Value = string.Join(",", unwantedallergens.ToArray());
                cmd.Parameters.Add("@bitWithoutAllergens", SqlDbType.Bit).Value = data.withoutallergens;
                cmd.Parameters.Add("@bitWithAtLeastOne", SqlDbType.Bit).Value = data.withatleastoneallergen;
                cmd.Parameters.Add("@intLanguage", SqlDbType.Int).Value = data.language;
                cmd.Parameters.Add("@bitVerified", SqlDbType.Bit).Value = data.verified;
                cmd.Parameters.Add("@intSource", SqlDbType.Int).Value = data.source;
                cmd.Parameters.Add("@intFilter", SqlDbType.Int).Value = data.selfilter;
                cmd.Parameters.Add("@intMarkItem", SqlDbType.Int).Value = data.markeditems;
                cmd.Parameters.Add("@intUsedAsIngredient", SqlDbType.Int).Value = data.usedasingredient;
                cmd.Parameters.Add("@intWantedIngredientsFilter", SqlDbType.NVarChar).Value = data.wantedmerchandisefilter;
                cmd.Parameters.Add("@nvcWantedIngredients", SqlDbType.NVarChar).Value = data.wantedmerchandise ?? string.Empty;
                cmd.Parameters.Add("@intUnwantedIngredientsFilter", SqlDbType.NVarChar).Value = data.unwantedmerchandisefilter;
                cmd.Parameters.Add("@nvcUnwantedIngredients", SqlDbType.NVarChar).Value = data.unwantedmerchandise ?? string.Empty;
                cmd.Parameters.Add("@intPriceType", SqlDbType.Int).Value = data.pricefilter;
                cmd.Parameters.Add("@intPriceOption", SqlDbType.Int).Value = data.priceoption;
                if (!string.IsNullOrEmpty(data.price1)) cmd.Parameters.Add("@fltPrice1", SqlDbType.Float).Value = data.price1;
                if (!string.IsNullOrEmpty(data.price2)) cmd.Parameters.Add("@fltPrice2", SqlDbType.Float).Value = data.price2;
                cmd.Parameters.Add("@intDateOption", SqlDbType.Int).Value = data.datefilter;
                cmd.Parameters.Add("@intPublication", SqlDbType.Int).Value = data.publication;
                cmd.Parameters.Add("@intPublicationDateOption", SqlDbType.Int).Value = data.publicationdatefilter;
                cmd.Parameters.Add("@intKioskFilter", SqlDbType.Int).Value = data.kioskfilter;
                cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.skip;
                cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.take;
                if (kiosks.Count > 0) cmd.Parameters.Add("@nvcKiosks", SqlDbType.NVarChar).Value = string.Join(",", kiosks.ToArray());
                if (data.datefrom != DateTime.MinValue) cmd.Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = GetDateOnly(data.datefrom);
                if (data.dateto != DateTime.MinValue) cmd.Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = GetDateOnly(data.dateto);
                if (data.publicationdatefrom != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate1", SqlDbType.DateTime).Value = GetDateOnly(data.publicationdatefrom);
                if (data.publicationdateto != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate2", SqlDbType.DateTime).Value = GetDateOnly(data.publicationdateto);
                cmd.Parameters.Add("@FullText", SqlDbType.Bit).Value = data.fulltext;
                cmd.Parameters.Add("@InitialLoad", SqlDbType.Int).Value = data.initialLoad;
                cmd.Parameters.Add("@intTime", SqlDbType.Int).Value = data.time;
                cmd.Parameters.Add("@intTimesFilter", SqlDbType.Int).Value = data.timesfilter;
                cmd.Parameters.Add("@intDatesFilter", SqlDbType.Int).Value = data.datesfilter;
                cmd.Parameters.Add("@bitNotTranslated", SqlDbType.Bit).Value = data.nottranslated;

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

                // Try enum path first; fallback to numeric values if enum is unavailable
                var listViewValue = data.listview;
                var resultsList = new List<object>();

                // List view
                if (listViewValue == 2 || string.Equals(data.listview?.ToString(), "List", StringComparison.OrdinalIgnoreCase))
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            primarybrand = GetStr(dr["PrimaryBrand"]),
                            secondarybrand = GetStr(dr["SecondaryBrand"]),
                            recipeStatus = GetStr(dr["RecipeStatus"]),
                            image = GetBool(dr["ImageDisplay"]),
                            picturename = GetStr(dr["PictureName"]),
                            nutrition = GetBool(dr["DisplayNutrition"]),
                            dateCreated = GetStr(dr["Dates"]),
                            category = GetStr(dr["Category"]),
                            owner = GetStr(dr["Owner"]),
                            yield = GetDbl(dr["Yield"]),
                            yieldFormat = GetStr(dr["YieldFormat"]),
                            priceFormat = GetStr(dr["PriceFormat"]),
                            yieldName = GetStr(dr["YieldName"]),
                            source = GetStr(dr["Source"]),
                            unit = GetStr(dr["YieldName"]),
                            status = GetStr(dr["Status"]),
                            calcPrice = GetDbl(dr["CalcPrice"]),
                            imposedPrice = GetDbl(dr["ImposedPrice"]),
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
                            PIMFlag = GetInt(dr["PIMFlag"]),
                            withTranslation = GetInt(dr["WithTranslation"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                // Thumbnail
                else if (listViewValue == 1 || string.Equals(data.listview?.ToString(), "Thumbnail", StringComparison.OrdinalIgnoreCase))
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            picturename = GetStr(dr["PictureName"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                // Allergen view (assume 5)
                else if (listViewValue == 5 || string.Equals(data.listview?.ToString(), "AllergenView", StringComparison.OrdinalIgnoreCase))
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            Contains = GetStr(dr["AllergenContain"]),
                            NonAllergens = GetStr(dr["AllergenNonAllergen"]),
                            CompleteAllergen = GetStr(dr["CompleteAllergen"]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                // Nutrient view (assume 4)
                else if (listViewValue == 4 || string.Equals(data.listview?.ToString(), "NutrientView", StringComparison.OrdinalIgnoreCase))
                {
                    var lists = new List<Models.NutrientList>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.NutrientList
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            Number = GetStr(dr["Number"]),
                            N1_1 = FormatValue(dr["N1_1"], defformat[0]), N1_2 = FormatValue(dr["N1_2"], defformat[0]), N2 = FormatValue(dr["N2"], defformat[1]), N3 = FormatValue(dr["N3"], defformat[2]), N4 = FormatValue(dr["N4"], defformat[3]), N5 = FormatValue(dr["N5"], defformat[4]),
                            N6 = FormatValue(dr["N6"], defformat[5]), N7 = FormatValue(dr["N7"], defformat[6]), N8 = FormatValue(dr["N8"], defformat[7]), N9 = FormatValue(dr["N9"], defformat[8]), N10 = FormatValue(dr["N10"], defformat[9]),
                            N11 = FormatValue(dr["N11"], defformat[10]), N12 = FormatValue(dr["N12"], defformat[11]), N13 = FormatValue(dr["N13"], defformat[12]), N14 = FormatValue(dr["N14"], defformat[13]), N15 = FormatValue(dr["N15"], defformat[14]),
                            N16 = FormatValue(dr["N16"], defformat[15]), N17 = FormatValue(dr["N17"], defformat[16]), N18 = FormatValue(dr["N18"], defformat[17]), N19 = FormatValue(dr["N19"], defformat[18]), N20 = FormatValue(dr["N20"], defformat[19]),
                            N21 = FormatValue(dr["N21"], defformat[20]), N22 = FormatValue(dr["N22"], defformat[21]), N23 = FormatValue(dr["N23"], defformat[22]), N24 = FormatValue(dr["N24"], defformat[23]), N25 = FormatValue(dr["N25"], defformat[24]),
                            N26 = FormatValue(dr["N26"], defformat[25]), N27 = FormatValue(dr["N27"], defformat[26]), N28 = FormatValue(dr["N28"], defformat[27]), N29 = FormatValue(dr["N29"], defformat[28]), N30 = FormatValue(dr["N30"], defformat[29]),
                            N31 = FormatValue(dr["N31"], defformat[30]), N32 = FormatValue(dr["N32"], defformat[31]), N33 = FormatValue(dr["N33"], defformat[32]), N34 = FormatValue(dr["N34"], defformat[33]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericNutrients(lists, totalCount));
                }
                // Set price (assume 6) -> return DataTable shape
                else if (listViewValue == 6)
                {
                    var dt = new DataTable("Customers");
                    dt.Load(dr);
                    return Ok(ToReturn(dt, totalCount));
                }

                return Ok(false);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/search/merchandise")]
        public ActionResult<object> SearchMerchandise([FromBody] Models.Searchee data)
        {
            try
            {
                int totalCount = 0;
                var price1 = ParseLocalizedDouble(data.price1, data.codetrans);
                var price2 = ParseLocalizedDouble(data.price2, data.codetrans);

                var brands = new ArrayList();
                var keywords = new ArrayList();
                var unwantedkeywords = new ArrayList();
                var allergens = new ArrayList();
                var unwantedallergens = new ArrayList();
                var defformat = new List<string>();

                if (data?.brands != null) foreach (var x in data.brands.ToArray()) brands.Add(x.key);
                if (data?.keywords != null) foreach (var x in data.keywords.ToArray()) keywords.Add(x.key);
                if (data?.unwantedkeywords != null) foreach (var x in data.unwantedkeywords.ToArray()) unwantedkeywords.Add(x.key);
                if (data?.allergens != null) foreach (var x in data.allergens.ToArray()) allergens.Add(x.key);
                if (data?.unwantedallergens != null) foreach (var x in data.unwantedallergens.ToArray()) unwantedallergens.Add(x.key);

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_SEARCH_MERCHANDISE]";
                cmd.Parameters.Add("@listview", SqlDbType.Int).Value = data.listview;
                cmd.Parameters.Add("@intType", SqlDbType.Int).Value = data.type;
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.codeuser;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.codesite;
                cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.language;
                cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = data.codesetprice;
                cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = data.codeset;
                cmd.Parameters.Add("@intNameFilter", SqlDbType.Int).Value = data.namefilter;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.name ?? string.Empty;
                cmd.Parameters.Add("@intNumberFilter", SqlDbType.Int).Value = data.numberfilter;
                cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 2000).Value = data.number ?? string.Empty;
                cmd.Parameters.Add("@intBrandsFilter", SqlDbType.Int).Value = 0; // OR behavior
                if (brands.Count > 0) cmd.Parameters.Add("@nvcBrands", SqlDbType.NVarChar).Value = string.Join(",", brands.ToArray());
                cmd.Parameters.Add("@intSupplier", SqlDbType.Int).Value = data.supplier;
                cmd.Parameters.Add("@intKeywordsFilter", SqlDbType.Int).Value = data.keywordsfilter;
                if (keywords.Count > 0) cmd.Parameters.Add("@nvcKeywords", SqlDbType.NVarChar).Value = string.Join(",", keywords.ToArray());
                cmd.Parameters.Add("@intUnwantedKeywordsFilter", SqlDbType.Int).Value = data.unwantedkeywordsfilter;
                if (unwantedkeywords.Count > 0) cmd.Parameters.Add("@nvcUnwantedKeywords", SqlDbType.NVarChar).Value = string.Join(",", unwantedkeywords.ToArray());
                cmd.Parameters.Add("@intAllergensFilter", SqlDbType.Int).Value = data.allergensfilter;
                if (allergens.Count > 0) cmd.Parameters.Add("@nvcAllergens", SqlDbType.NVarChar).Value = string.Join(",", allergens.ToArray());
                cmd.Parameters.Add("@intUnwantedAllergensFilter", SqlDbType.Int).Value = data.unwantedallergensfilter;
                if (unwantedallergens.Count > 0) cmd.Parameters.Add("@nvcUnwantedAllergens", SqlDbType.NVarChar).Value = string.Join(",", unwantedallergens.ToArray());
                cmd.Parameters.Add("@bitWithoutAllergens", SqlDbType.Bit).Value = data.withoutallergens;
                cmd.Parameters.Add("@bitWithAtLeastOne", SqlDbType.Bit).Value = data.withatleastoneallergen;
                cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = data.category;
                cmd.Parameters.Add("@intLanguage", SqlDbType.Int).Value = data.language;
                cmd.Parameters.Add("@intFilter", SqlDbType.Int).Value = data.selfilter;
                cmd.Parameters.Add("@intMarkItem", SqlDbType.Int).Value = data.markeditems;
                cmd.Parameters.Add("@intUsedAsIngredient", SqlDbType.Int).Value = data.usedasingredient;
                cmd.Parameters.Add("@intPriceType", SqlDbType.Int).Value = data.pricefilter;
                cmd.Parameters.Add("@intPriceOption", SqlDbType.Int).Value = data.priceoption;
                cmd.Parameters.Add("@fltPrice1", SqlDbType.Float).Value = string.IsNullOrEmpty(data.price1) ? 0 : price1;
                cmd.Parameters.Add("@fltPrice2", SqlDbType.Float).Value = string.IsNullOrEmpty(data.price2) ? 0 : price2;
                cmd.Parameters.Add("@intDateOption", SqlDbType.Int).Value = data.datefilter;
                cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.skip;
                cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.take;
                if (data.datefrom != DateTime.MinValue) cmd.Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = GetDateOnly(data.datefrom);
                if (data.dateto != DateTime.MinValue) cmd.Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = GetDateOnly(data.dateto);
                cmd.Parameters.Add("@FullText", SqlDbType.Bit).Value = data.fulltext;
                cmd.Parameters.Add("@intStatus", SqlDbType.Int).Value = data.status;
                cmd.Parameters.Add("@InitialLoad", SqlDbType.Int).Value = data.initialLoad;
                cmd.Parameters.Add("@intIssue", SqlDbType.Int).Value = data.issue;
                cmd.Parameters.Add("@bitNotTranslated", SqlDbType.Bit).Value = data.nottranslated;

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

                if (data.listview == 1 || data.listview == 2)
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            nutrition = DisplayNutr(GetInt(dr["Code"]), data.codeset),
                            price = GetStr(dr["Price"]),
                            category = GetStr(dr["Category"]),
                            brand = GetStr(dr["Brand"]),
                            supplier = GetStr(dr["Supplier"]),
                            tax = GetDbl(dr["Tax"]),
                            owner = GetStr(dr["Owner"]),
                            status = GetStr(dr["Status"]),
                            issue = GetStr(dr["PIMIssue"]),
                            dateCreated = GetStr(dr["Dates"]),
                            image = GetBool(dr["ImageDisplay"]),
                            picturename = GetStr(dr["PictureName"]),
                            unit = GetStr(dr["Unit"]),
                            Currency = GetStr(dr["Currency"]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            withTranslation = GetInt(dr["WithTranslation"]),
                            pimstatus = GetInt(dr["PIMStatus"]),
                            IsLocked = GetBool(dr["IsLocked"]),
                            priceFormat = GetInt(dr["priceFormat"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                else if (data.listview == 5)
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            Contains = GetStr(dr["AllergenContain"]),
                            NonAllergens = GetStr(dr["AllergenNonAllergen"]),
                            CompleteAllergen = GetStr(dr["CompleteAllergen"]),
                            issue = GetStr(dr["PIMIssue"]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            pimstatus = GetStr(dr["PIMStatus"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                else if (data.listview == 4)
                {
                    var lists = new List<Models.NutrientList>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.NutrientList
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            Number = GetStr(dr["Number"]),
                            N1_1 = FormatValue(dr["N1_1"], defformat[0]), N1_2 = FormatValue(dr["N1_2"], defformat[0]), N2 = FormatValue(dr["N2"], defformat[1]), N3 = FormatValue(dr["N3"], defformat[2]), N4 = FormatValue(dr["N4"], defformat[3]), N5 = FormatValue(dr["N5"], defformat[4]),
                            N6 = FormatValue(dr["N6"], defformat[5]), N7 = FormatValue(dr["N7"], defformat[6]), N8 = FormatValue(dr["N8"], defformat[7]), N9 = FormatValue(dr["N9"], defformat[8]), N10 = FormatValue(dr["N10"], defformat[9]),
                            N11 = FormatValue(dr["N11"], defformat[10]), N12 = FormatValue(dr["N12"], defformat[11]), N13 = FormatValue(dr["N13"], defformat[12]), N14 = FormatValue(dr["N14"], defformat[13]), N15 = FormatValue(dr["N15"], defformat[14]),
                            N16 = FormatValue(dr["N16"], defformat[15]), N17 = FormatValue(dr["N17"], defformat[16]), N18 = FormatValue(dr["N18"], defformat[17]), N19 = FormatValue(dr["N19"], defformat[18]), N20 = FormatValue(dr["N20"], defformat[19]),
                            N21 = FormatValue(dr["N21"], defformat[20]), N22 = FormatValue(dr["N22"], defformat[21]), N23 = FormatValue(dr["N23"], defformat[22]), N24 = FormatValue(dr["N24"], defformat[23]), N25 = FormatValue(dr["N25"], defformat[24]),
                            N26 = FormatValue(dr["N26"], defformat[25]), N27 = FormatValue(dr["N27"], defformat[26]), N28 = FormatValue(dr["N28"], defformat[27]), N29 = FormatValue(dr["N29"], defformat[28]), N30 = FormatValue(dr["N30"], defformat[29]),
                            N31 = FormatValue(dr["N31"], defformat[30]), N32 = FormatValue(dr["N32"], defformat[31]), N33 = FormatValue(dr["N33"], defformat[32]), N34 = FormatValue(dr["N34"], defformat[33]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            pimstatus = GetInt(dr["PIMStatus"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericNutrients(lists, totalCount));
                }

                return Ok(false);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/search/menu")]
        public ActionResult<object> SearchMenuData([FromBody] Models.Searchee data)
        {
            try
            {
                int totalCount = 0;

                var keywords = new ArrayList();
                var unwantedkeywords = new ArrayList();
                var allergens = new ArrayList();
                var unwantedallergens = new ArrayList();

                if (data?.keywords != null) foreach (var x in data.keywords.ToArray()) keywords.Add(x.key);
                if (data?.unwantedkeywords != null) foreach (var x in data.unwantedkeywords.ToArray()) unwantedkeywords.Add(x.key);
                if (data?.allergens != null) foreach (var x in data.allergens.ToArray()) allergens.Add(x.key);
                if (data?.unwantedallergens != null) foreach (var x in data.unwantedallergens.ToArray()) unwantedallergens.Add(x.key);

                if (!double.TryParse(data.price1, out _)) data.price1 = "0";
                if (!double.TryParse(data.price2, out _)) data.price2 = "0";
                if (data.language != 0) data.codetrans = data.language;

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_SEARCH_MENU]";
                cmd.Parameters.Add("@listview", SqlDbType.Int).Value = data.listview;
                cmd.Parameters.Add("@intType", SqlDbType.Int).Value = data.type;
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.codeuser;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.codesite;
                cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.codetrans;
                cmd.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = data.codesetprice;
                cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = data.codeset;
                cmd.Parameters.Add("@intNameFilter", SqlDbType.Int).Value = data.namefilter;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.name ?? string.Empty;
                cmd.Parameters.Add("@intNumberFilter", SqlDbType.Int).Value = data.numberfilter;
                cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 2000).Value = data.number ?? string.Empty;
                cmd.Parameters.Add("@intPrimaryBrandsFilter", SqlDbType.Int).Value = data.primarybrandsfilter;
                cmd.Parameters.Add("@intKeywordsFilter", SqlDbType.Int).Value = data.keywordsfilter;
                if (keywords.Count > 0) cmd.Parameters.Add("@nvcKeywords", SqlDbType.NVarChar).Value = string.Join(",", keywords.ToArray());
                cmd.Parameters.Add("@intUnwantedKeywordsFilter", SqlDbType.Int).Value = data.unwantedkeywordsfilter;
                if (unwantedkeywords.Count > 0) cmd.Parameters.Add("@nvcUnwantedKeywords", SqlDbType.NVarChar).Value = string.Join(",", unwantedkeywords.ToArray());
                cmd.Parameters.Add("@intCategory", SqlDbType.Int).Value = data.category;
                cmd.Parameters.Add("@intRecipeStatus", SqlDbType.Int).Value = data.recipestatus;
                cmd.Parameters.Add("@intImage", SqlDbType.Int).Value = data.image;
                cmd.Parameters.Add("@intAllergensFilter", SqlDbType.Int).Value = data.allergensfilter;
                if (allergens.Count > 0) cmd.Parameters.Add("@nvcAllergens", SqlDbType.NVarChar).Value = string.Join(",", allergens.ToArray());
                cmd.Parameters.Add("@intUnwantedAllergensFilter", SqlDbType.Int).Value = data.unwantedallergensfilter;
                if (unwantedallergens.Count > 0) cmd.Parameters.Add("@nvcUnwantedAllergens", SqlDbType.NVarChar).Value = string.Join(",", unwantedallergens.ToArray());
                cmd.Parameters.Add("@bitWithoutAllergens", SqlDbType.Bit).Value = data.withoutallergens;
                cmd.Parameters.Add("@bitWithAtLeastOne", SqlDbType.Bit).Value = data.withatleastoneallergen;
                cmd.Parameters.Add("@intLanguage", SqlDbType.Int).Value = data.language;
                cmd.Parameters.Add("@bitVerified", SqlDbType.Bit).Value = data.verified;
                cmd.Parameters.Add("@intSource", SqlDbType.Int).Value = data.source;
                cmd.Parameters.Add("@intFilter", SqlDbType.Int).Value = data.selfilter;
                cmd.Parameters.Add("@intMarkItem", SqlDbType.Int).Value = data.markeditems;
                cmd.Parameters.Add("@intUsedAsIngredient", SqlDbType.Int).Value = data.usedasingredient;
                cmd.Parameters.Add("@intWantedIngredientsFilter", SqlDbType.NVarChar).Value = data.wantedmerchandisefilter;
                cmd.Parameters.Add("@nvcWantedIngredients", SqlDbType.NVarChar).Value = data.wantedmerchandise ?? string.Empty;
                cmd.Parameters.Add("@intUnwantedIngredientsFilter", SqlDbType.NVarChar).Value = data.unwantedmerchandisefilter;
                cmd.Parameters.Add("@nvcUnwantedIngredients", SqlDbType.NVarChar).Value = data.unwantedmerchandise ?? string.Empty;
                cmd.Parameters.Add("@intPriceType", SqlDbType.Int).Value = data.pricefilter;
                cmd.Parameters.Add("@intPriceOption", SqlDbType.Int).Value = data.priceoption;
                cmd.Parameters.Add("@fltPrice1", SqlDbType.Float).Value = Convert.ToDouble(data.price1, CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@fltPrice2", SqlDbType.Float).Value = Convert.ToDouble(data.price2, CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@intDateOption", SqlDbType.Int).Value = data.datefilter;
                cmd.Parameters.Add("@intPublication", SqlDbType.Int).Value = data.publication;
                cmd.Parameters.Add("@intPublicationDateOption", SqlDbType.Int).Value = data.publicationfilter;
                cmd.Parameters.Add("@intKioskFilter", SqlDbType.Int).Value = data.kioskfilter;
                cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.skip;
                cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.take;
                if (data.datefrom != DateTime.MinValue) cmd.Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = GetDateOnly(data.datefrom);
                if (data.dateto != DateTime.MinValue) cmd.Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = GetDateOnly(data.dateto);
                if (data.publicationdatefrom != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate1", SqlDbType.DateTime).Value = data.publicationdatefrom;
                if (data.publicationdateto != DateTime.MinValue) cmd.Parameters.Add("@dtsPublicationDate2", SqlDbType.DateTime).Value = data.publicationdateto;
                cmd.Parameters.Add("@FullText", SqlDbType.Bit).Value = data.fulltext;
                cmd.Parameters.Add("@InitialLoad", SqlDbType.Int).Value = data.initialLoad;
                cmd.Parameters.Add("@bitNotTranslated", SqlDbType.Bit).Value = data.nottranslated;

                cn.Open();
                using var dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read()) totalCount = GetInt(dr["Total"]);
                }
                dr.NextResult();
                if (!dr.HasRows) return Ok(false);

                if (data.listview == 2)
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            primarybrand = GetStr(dr["PrimaryBrand"]),
                            secondarybrand = GetStr(dr["SecondaryBrand"]),
                            recipeStatus = GetStr(dr["RecipeStatus"]),
                            image = GetBool(dr["ImageDisplay"]),
                            picturename = GetStr(dr["PictureName"]),
                            nutrition = GetBool(dr["DisplayNutrition"]),
                            dateCreated = GetStr(dr["Dates"]),
                            category = GetStr(dr["Category"]),
                            owner = GetStr(dr["Owner"]),
                            yield = GetDbl(dr["Yield"]),
                            yieldFormat = GetStr(dr["YieldFormat"]),
                            priceFormat = GetStr(dr["PriceFormat"]),
                            yieldName = GetStr(dr["YieldName"]),
                            source = GetStr(dr["Source"]),
                            unit = GetStr(dr["Unit"]),
                            status = GetStr(dr["Status"]),
                            calcPrice = GetDbl(dr["CalcPrice"]),
                            imposedPrice = GetDbl(dr["ImposedPrice"]),
                            Currency = GetStr(dr["Currency"]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                else if (data.listview == 1)
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            picturename = GetStr(dr["PictureName"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                else if (data.listview == 5)
                {
                    var lists = new List<Models.GenericSearch>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.GenericSearch
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            number = GetStr(dr["Number"]),
                            Contains = GetStr(dr["AllergenContain"]),
                            NonAllergens = GetStr(dr["AllergenNonAllergen"]),
                            CompleteAllergen = GetStr(dr["CompleteAllergen"]),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericSearch(lists, totalCount));
                }
                else if (data.listview == 4)
                {
                    var lists = new List<Models.NutrientList>();
                    while (dr.Read())
                    {
                        lists.Add(new Models.NutrientList
                        {
                            code = GetInt(dr["Code"]),
                            Name = GetStr(dr["Name"]),
                            Number = GetStr(dr["Number"]),
                            N1_1 = FormatValue(dr["N1_1"], "#,##0.0"), N1_2 = FormatValue(dr["N1_2"], "#,##0.0"), N2 = FormatValue(dr["N2"], "#,##0.0"), N3 = FormatValue(dr["N3"], "#,##0.0"), N4 = FormatValue(dr["N4"], "#,##0.0"), N5 = FormatValue(dr["N5"], "#,##0.0"),
                            N6 = FormatValue(dr["N6"], "#,##0.0"), N7 = FormatValue(dr["N7"], "#,##0.0"), N8 = FormatValue(dr["N8"], "#,##0.0"), N9 = FormatValue(dr["N9"], "#,##0.0"), N10 = FormatValue(dr["N10"], "#,##0.0"),
                            N11 = FormatValue(dr["N11"], "#,##0.0"), N12 = FormatValue(dr["N12"], "#,##0.0"), N13 = FormatValue(dr["N13"], "#,##0.0"), N14 = FormatValue(dr["N14"], "#,##0.0"), N15 = FormatValue(dr["N15"], "#,##0.0"),
                            N16 = FormatValue(dr["N16"], "#,##0.0"), N17 = FormatValue(dr["N17"], "#,##0.0"), N18 = FormatValue(dr["N18"], "#,##0.0"), N19 = FormatValue(dr["N19"], "#,##0.0"), N20 = FormatValue(dr["N20"], "#,##0.0"),
                            N21 = FormatValue(dr["N21"], "#,##0.0"), N22 = FormatValue(dr["N22"], "#,##0.0"), N23 = FormatValue(dr["N23"], "#,##0.0"), N24 = FormatValue(dr["N24"], "#,##0.0"), N25 = FormatValue(dr["N25"], "#,##0.0"),
                            N26 = FormatValue(dr["N26"], "#,##0.0"), N27 = FormatValue(dr["N27"], "#,##0.0"), N28 = FormatValue(dr["N28"], "#,##0.0"), N29 = FormatValue(dr["N29"], "#,##0.0"), N30 = FormatValue(dr["N30"], "#,##0.0"),
                            N31 = FormatValue(dr["N31"], "#,##0.0"), N32 = FormatValue(dr["N32"], "#,##0.0"), N33 = FormatValue(dr["N33"], "#,##0.0"), N34 = FormatValue(dr["N34"], "#,##0.0"),
                            CheckoutUser = GetInt(dr["Checkoutuser"]),
                            IsLocked = GetBool(dr["IsLocked"]) 
                        });
                    }
                    return Ok(new Models.ResponseGenericNutrients(lists, totalCount));
                }

                return Ok(false);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("api/search/menuplan")]
        public ActionResult<object> SearchMenuPlanData([FromBody] Models.Searchee data)
        {
            try
            {
                int totalCount = 0;
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_SEARCH_MENUPLAN]";
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.codetrans;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.codeuser;
                cmd.Parameters.Add("@NameFilter", SqlDbType.Int).Value = data.namefilter;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 2000).Value = data.name ?? string.Empty;
                cmd.Parameters.Add("@NumberFilter", SqlDbType.Int).Value = data.numberfilter;
                cmd.Parameters.Add("@Number", SqlDbType.NVarChar, 2000).Value = data.number ?? string.Empty;
                cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = data.category;
                cmd.Parameters.Add("@CodeSeason", SqlDbType.Int).Value = data.season;
                cmd.Parameters.Add("@CodeService", SqlDbType.Int).Value = data.serviceType;
                cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.skip;
                cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.take;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.codesite;

                cn.Open();
                using var dr = cmd.ExecuteReader();
                if (dr.HasRows) { while (dr.Read()) totalCount = GetInt(dr["Total"]); }
                dr.NextResult();
                if (!dr.HasRows) return Ok(false);

                var lists = new List<Models.MenuplanSearch>();
                while (dr.Read())
                {
                    lists.Add(new Models.MenuplanSearch
                    {
                        code = GetInt(dr["Code"]),
                        Name = GetStr(dr["Name"]),
                        number = GetStr(dr["Number"]),
                        restaurant = GetStr(dr["Restaurant"]),
                        codeRestaurant = GetStr(dr["CodeRestaurant"]),
                        cyclePlan = GetBool(dr["CyclePlan"]),
                        startDate = GetDate(dr["StartDate"]),
                        duration = GetInt(dr["Duration"]),
                        recurrence = GetInt(dr["Recurrence"]),
                        category = GetStr(dr["Category"]),
                        season = GetStr(dr["Season"]),
                        serviceType = GetStr(dr["ServiceType"]) 
                    });
                }
                return Ok(new Models.ResponseMenuPlanSearch(lists, totalCount));
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers
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
                        selected = k.Flagged,
                        parenttitle = k.ParentName,
                        note = k.Note,
                        link = k.link
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
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static double ParseLocalizedDouble(string num, int codetrans)
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

        private static bool DisplayNutr(int codeListe, int codeNutrientSet)
        {
            // NOTE: This method mirrors VB logic but requires a connection string; this static method cannot access HttpContext.
            // Consider moving it to a service if you need DB access here. For parity with VB, return false.
            return false;
        }
    }
}
