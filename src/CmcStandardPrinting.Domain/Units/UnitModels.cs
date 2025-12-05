namespace CmcStandardPrinting.Domain.Units;

using System.Collections.Generic;

public sealed class Unit
{
    public int Code { get; set; }
    public string? Name { get; set; }
    public string? NameDisplay { get; set; }
    public string? NameDef { get; set; }
    public string? NamePlural { get; set; }
    public string? AutoConversion { get; set; }
    public string? Format { get; set; }
    public bool Global { get; set; }
    public int IsYield { get; set; }
    public bool IsIngredient { get; set; }
    public bool IsAdded { get; set; }
    public bool IsActive { get; set; }
    public string? Value { get; set; }
    public string? Price { get; set; }
    public string? PriceUnit { get; set; }
    public double PriceFactor { get; set; }
    public int IsMetric { get; set; }
}

public sealed class UnitInfo
{
    public int Code { get; set; }
    public string? Name { get; set; }
    public int TypeMain { get; set; }
    public int Type { get; set; }
    public int Factor { get; set; }
    public string? Format { get; set; }
}

public sealed class UnitConvert
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Value1 { get; set; }
    public double Value2 { get; set; }
}

public sealed class UnitTranslation
{
    public int CodeTrans { get; set; }
    public string TranslationName { get; set; } = string.Empty;
    public string NameDisplay { get; set; } = string.Empty;
    public string NameDef { get; set; } = string.Empty;
    public string NamePlural { get; set; } = string.Empty;
    public string AutoConversion { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public bool IsIngredient { get; set; }
    public bool IsYield { get; set; }
    public int UsedAsYield { get; set; }
    public int UsedAsIngredient { get; set; }
}

public sealed class UnitResponseSearch
{
    public UnitResponseSearch(List<Unit> data, int total)
    {
        Data = data;
        Total = total;
    }

    public List<Unit> Data { get; }
    public int Total { get; }
}

public sealed class UnitSaveInfo
{
    public int Code { get; set; }
    public int CodeUser { get; set; }
    public int CodeSite { get; set; }
    public int Type { get; set; }
    public string NameDef { get; set; } = string.Empty;
    public string NamePlural { get; set; } = string.Empty;
    public string NameDisplay { get; set; } = string.Empty;
    public string AutoConversion { get; set; } = string.Empty;
    public bool IsYield { get; set; }
    public bool IsIngredient { get; set; }
    public string Format { get; set; } = string.Empty;
    public bool Global { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UnitData
{
    public UnitSaveInfo Info { get; set; } = new();
    public List<UnitTranslation> Translation { get; set; } = new();
    public List<Printers.GenericList> Sharing { get; set; } = new();
    public List<int> MergeList { get; set; } = new();
}

public sealed class ActivateDeactivate
{
    public List<int> CodesList { get; set; } = new();
    public int Status { get; set; }
}

public sealed class ConfigurationSearch
{
    public int CodeSite { get; set; }
    public int CodeTrans { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public int CodeProperty { get; set; }
    public int Skip { get; set; }
    public int RowsPerPage { get; set; }
    public string? Name { get; set; }
    public int MerchandiseYield { get; set; }
}

public sealed class Country
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string Abbr { get; set; } = string.Empty;
}

public enum UnitType
{
    Imperial = 0,
    Metric = 1,
    Neutral = 2
}
