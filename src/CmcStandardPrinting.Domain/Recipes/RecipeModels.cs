using System.Collections.Generic;

namespace CmcStandardPrinting.Domain.Recipes;

public sealed class RecipeAttachment
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public sealed class RecipeUsedAsIngredient
{
    public int Code { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class IngredientWeight
{
    public int Code { get; set; }
    public int CodeUnit { get; set; }
    public double Quantity { get; set; }
    public int CodeSetPrice { get; set; }
}

public sealed class IngredientWeightList
{
    public List<IngredientWeight> Data { get; set; } = new();
    public int DisplayCodeUnit { get; set; }
}

public sealed class RecipeCheckout
{
    public int CodeListe { get; set; }
    public int CodeUser { get; set; }
}

public sealed class Label
{
    public int Code { get; set; }
    public int CodeListe { get; set; }
    public string DeclarationName { get; set; } = string.Empty;
    public string SpecificDetermination { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public double Consumption { get; set; }
    public double Sold { get; set; }
    public string Composition { get; set; } = string.Empty;
    public double Weight { get; set; }
    public int Unit { get; set; }
    public double Price { get; set; }
    public double PriceFor { get; set; }
    public double Calculated { get; set; }
    public int Note1 { get; set; }
    public int Note2 { get; set; }
    public int Note3 { get; set; }
    public int Certification { get; set; }
    public int CountryOfProduction { get; set; }
    public int PackagingMethod { get; set; }
    public int Storage { get; set; }
}

public sealed class LabelComposition
{
    public int CodeListe { get; set; }
    public int CompoType { get; set; }
    public int CodeTrans { get; set; }
    public string ImposedComposition { get; set; } = string.Empty;
    public bool IsValidated { get; set; }
}

public sealed class ListeFiles
{
    public int Code { get; set; }
    public string Pictures { get; set; } = string.Empty;
    public string Videos { get; set; } = string.Empty;
}
