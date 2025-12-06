using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;

namespace CmcStandardPrinting.Domain.Ingredients;

public sealed class IngredientListResponse
{
    public IngredientListResponse()
    {
    }

    public IngredientListResponse(List<Ingredient> data, int total)
    {
        Data = data;
        Total = total;
    }

    public List<Ingredient> Data { get; set; } = new();

    public int Total { get; set; }
}

public sealed class Ingredient
{
    public int CodeListe { get; set; }

    public int Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public double Price { get; set; }

    public string UnitName { get; set; } = string.Empty;

    public string UnitMetric { get; set; } = string.Empty;

    public string UnitImperial { get; set; } = string.Empty;

    public int CodeUnit { get; set; }

    public int CodeUnitMetric { get; set; }

    public int CodeUnitImperial { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string SourceName { get; set; } = string.Empty;

    public string SupplierName { get; set; } = string.Empty;

    public int CodeBrand { get; set; }

    public string BrandName { get; set; } = string.Empty;

    public int Wastage1 { get; set; }

    public int Wastage2 { get; set; }

    public int Wastage3 { get; set; }

    public int Wastage4 { get; set; }

    public int Wastage5 { get; set; }

    public int WastageTotal { get; set; }

    public object? Status { get; set; }

    public double ImposedPrice { get; set; }

    public int Constant { get; set; }

    public string Preparation { get; set; } = string.Empty;

    public string Allprice { get; set; } = string.Empty;

    public int withTranslation { get; set; }

    public bool isLocked { get; set; }

    public double yieldIng { get; set; }

    public int CodeCategory { get; set; }

    public int CodeUser { get; set; }
}

public sealed class IngredientInfo
{
    public int CodeListe { get; set; }

    public int CodeSetPrice { get; set; }

    public int CodeUnit { get; set; }

    public string UnitName { get; set; } = string.Empty;

    public bool IsNewUnit { get; set; }

    public double Price { get; set; }

    public int CodeSite { get; set; }

    public int CodeUser { get; set; }

    public int CodeTrans { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public int CodeCategory { get; set; }

    public int Wastage1 { get; set; }

    public int Wastage2 { get; set; }

    public int Wastage3 { get; set; }

    public int Wastage4 { get; set; }
}

public sealed class IngredientData
{
    public IngredientInfo Info { get; set; } = new();

    public List<SharingItem> Sharing { get; set; } = new();
}

public sealed class IngredientOnePrice
{
    public int Code { get; set; }

    public object? CodeUnit { get; set; }

    public object? Position { get; set; }

    public object? Price { get; set; }

    public object? CodeSetPrice { get; set; }
}
