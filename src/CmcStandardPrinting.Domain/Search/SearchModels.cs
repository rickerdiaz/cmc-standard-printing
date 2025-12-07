namespace CmcStandardPrinting.Domain.Search;

using System;
using System.Collections.Generic;

public sealed class Searchee
{
    public int Take { get; set; }
    public int Skip { get; set; }
    public int ListView { get; set; }
    public int Type { get; set; }
    public int CodeUser { get; set; }
    public int CodeSite { get; set; }
    public int CodeTrans { get; set; }
    public int CodeSetPrice { get; set; }
    public int CodeSet { get; set; }
    public int NameFilter { get; set; }
    public string? Name { get; set; }
    public int NumberFilter { get; set; }
    public string? Number { get; set; }
    public int TimesFilter { get; set; }
    public int Time { get; set; }
    public int DatesFilter { get; set; }
    public int Brand { get; set; }
    public int Supplier { get; set; }
    public int BrandsFilter { get; set; }
    public List<GenericArrays>? Brands { get; set; }
    public int PrimaryBrandsFilter { get; set; }
    public List<GenericArrays>? PrimaryBrands { get; set; }
    public int UnwantedPrimaryBrandsFilter { get; set; }
    public List<GenericArrays>? UnwantedPrimaryBrands { get; set; }
    public int SecondaryBrandsFilter { get; set; }
    public List<GenericArrays>? SecondaryBrands { get; set; }
    public int UnwantedSecondaryBrandsFilter { get; set; }
    public List<GenericArrays>? UnwantedSecondaryBrands { get; set; }
    public int KeywordsFilter { get; set; }
    public List<GenericArrays>? Keywords { get; set; }
    public int UnwantedKeywordsFilter { get; set; }
    public List<GenericArrays>? UnwantedKeywords { get; set; }
    public int Category { get; set; }
    public int RecipeStatus { get; set; }
    public int Image { get; set; }
    public int AllergensFilter { get; set; }
    public List<GenericArrays>? Allergens { get; set; }
    public int UnwantedAllergensFilter { get; set; }
    public List<GenericArrays>? UnwantedAllergens { get; set; }
    public bool FullText { get; set; }
    public bool WithoutAllergens { get; set; }
    public bool WithAtLeastOneAllergen { get; set; }
    public int Language { get; set; }
    public bool Verified { get; set; }
    public bool NotTranslated { get; set; }
    public int Source { get; set; }
    public int SelFilter { get; set; }
    public int MarkedItems { get; set; }
    public int UsedAsIngredient { get; set; }
    public int WantedMerchandiseFilter { get; set; }
    public string? WantedMerchandise { get; set; }
    public int UnwantedMerchandiseFilter { get; set; }
    public string? UnwantedMerchandise { get; set; }
    public int PriceFilter { get; set; }
    public int PriceOption { get; set; }
    public string? Price1 { get; set; }
    public string? Price2 { get; set; }
    public int DateFilter { get; set; }
    public int Publication { get; set; }
    public int PublicationDateFilter { get; set; }
    public DateTime PublicationDateFrom { get; set; }
    public DateTime PublicationDateTo { get; set; }
    public int KioskFilter { get; set; }
    public List<GenericArrays>? Kiosks { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string? WantedItems { get; set; }
    public string? UnwantedItems { get; set; }
    public int InitialLoad { get; set; }
    public int Status { get; set; }
    public int Issue { get; set; }
    public string? Issues { get; set; }
    public int Season { get; set; }
    public string? ServiceType { get; set; }
}

public sealed class GenericArrays
{
    public int Key { get; set; }
    public string Title { get; set; } = string.Empty;
}

public sealed class GenericSearch
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Number { get; set; }
    public string? PrimaryBrand { get; set; }
    public string? SecondaryBrand { get; set; }
    public string? RecipeStatus { get; set; }
    public bool Image { get; set; }
    public string? PictureName { get; set; }
    public bool Nutrition { get; set; }
    public string? DateCreated { get; set; }
    public string? Category { get; set; }
    public string? Owner { get; set; }
    public double Yield { get; set; }
    public string? YieldFormat { get; set; }
    public string? PriceFormat { get; set; }
    public string? YieldName { get; set; }
    public string? Source { get; set; }
    public string? Unit { get; set; }
    public string? Status { get; set; }
    public double CalcPrice { get; set; }
    public double ImposedPrice { get; set; }
    public string? Currency { get; set; }
    public int CheckoutUser { get; set; }
    public double FoodCost { get; set; }
    public double FoodCostPercent { get; set; }
    public double GrossMargin { get; set; }
    public double GrossMarginPercent { get; set; }
    public double NetMargin { get; set; }
    public double NetMarginPercent { get; set; }
    public double ImposedSellingPriceWOTax { get; set; }
    public double ImposedSellingPriceWTax { get; set; }
    public int PimFlag { get; set; }
    public DateTime DateTested { get; set; }
    public int WithTranslation { get; set; }
    public bool IsLocked { get; set; }
    public string? Contains { get; set; }
    public string? NonAllergens { get; set; }
    public string? CompleteAllergen { get; set; }
    public string? Brand { get; set; }
    public string? Supplier { get; set; }
    public double Tax { get; set; }
    public string? Issue { get; set; }
    public bool Flagged { get; set; }
    public int PimStatus { get; set; }
}

public sealed class ResponseGenericSearch
{
    public ResponseGenericSearch(List<GenericSearch> data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
    }

    public List<GenericSearch> Data { get; set; }
    public int TotalCount { get; set; }
}

public sealed class NutrientList
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Number { get; set; }
    public string? N1_1 { get; set; }
    public string? N1_2 { get; set; }
    public string? N2 { get; set; }
    public string? N3 { get; set; }
    public string? N4 { get; set; }
    public string? N5 { get; set; }
    public string? N6 { get; set; }
    public string? N7 { get; set; }
    public string? N8 { get; set; }
    public string? N9 { get; set; }
    public string? N10 { get; set; }
    public string? N11 { get; set; }
    public string? N12 { get; set; }
    public string? N13 { get; set; }
    public string? N14 { get; set; }
    public string? N15 { get; set; }
    public string? N16 { get; set; }
    public string? N17 { get; set; }
    public string? N18 { get; set; }
    public string? N19 { get; set; }
    public string? N20 { get; set; }
    public string? N21 { get; set; }
    public string? N22 { get; set; }
    public string? N23 { get; set; }
    public string? N24 { get; set; }
    public string? N25 { get; set; }
    public string? N26 { get; set; }
    public string? N27 { get; set; }
    public string? N28 { get; set; }
    public string? N29 { get; set; }
    public string? N30 { get; set; }
    public string? N31 { get; set; }
    public string? N32 { get; set; }
    public string? N33 { get; set; }
    public string? N34 { get; set; }
    public int CheckoutUser { get; set; }
    public bool IsLocked { get; set; }
}

public sealed class ResponseGenericNutrients
{
    public ResponseGenericNutrients(List<NutrientList> data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
    }

    public List<NutrientList> Data { get; set; }
    public int TotalCount { get; set; }
}

public sealed class MenuplanSearch
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Number { get; set; }
    public string Restaurant { get; set; } = string.Empty;
    public string CodeRestaurant { get; set; } = string.Empty;
    public bool CyclePlan { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int Recurrence { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
}

public sealed class ResponseMenuPlanSearch
{
    public ResponseMenuPlanSearch(List<MenuplanSearch> data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
    }

    public List<MenuplanSearch> Data { get; set; }
    public int TotalCount { get; set; }
}
