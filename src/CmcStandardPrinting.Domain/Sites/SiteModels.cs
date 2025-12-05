namespace CmcStandardPrinting.Domain.Sites;

using System.Collections.Generic;

public sealed class GenericListItem
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? Value2 { get; set; }
    public string? Name { get; set; }
}

public sealed class GenericItem
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class UsedLanguage
{
    public int CodeRef { get; set; }
    public string Language { get; set; } = string.Empty;
}

public sealed class SiteInfo
{
    public string Name { get; set; } = string.Empty;
    public string RefName { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string SiteLevel { get; set; } = string.Empty;
}

public sealed class SiteTranslation
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
    public int CodeDict { get; set; }
}

public sealed class SiteData
{
    public SiteInfo Info { get; set; } = new();
    public List<SiteTranslation> Translation { get; set; } = new();
    public List<GenericListItem> Autonumber { get; set; } = new();
    public string Config { get; set; } = string.Empty;
    public List<GenericListItem> NutrientSet { get; set; } = new();
    public List<GenericListItem> SetOfPrice { get; set; } = new();
    public List<Tax> Tax { get; set; } = new();
    public List<Unit> Units { get; set; } = new();
}

public sealed class Tax
{
    public int TaxCode { get; set; }
    public double TaxValue { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public bool Global { get; set; }
}

public sealed class Unit
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Type { get; set; }
    public int TypeMain { get; set; }
    public double FactorToMain { get; set; }
    public int IsMetric { get; set; }
    public bool IsIngredient { get; set; }
    public bool IsYield { get; set; }
    public string Format { get; set; } = string.Empty;
}
