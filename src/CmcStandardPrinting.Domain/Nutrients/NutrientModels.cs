using System.Collections.Generic;

namespace CmcStandardPrinting.Domain.Nutrients;

public sealed class RecipeNutrition
{
    public int Id { get; set; }
    public int Nutr_No { get; set; }
    public int Position { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public double Value { get; set; }
    public double Imposed { get; set; }
    public double Percent { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int GDA { get; set; }
    public int CodeNutrientSet { get; set; }
    public string NutrientSet { get; set; } = string.Empty;
    public bool DisplayNutrition { get; set; }
    public bool Display { get; set; }
    public int ImposedType { get; set; }
    public string PortionSize { get; set; } = string.Empty;
    public string NutritionBasis { get; set; } = string.Empty;
}

public sealed class NutrientDefinition
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
}

public sealed class NutrientResponse
{
    public List<NutrientDefinition> NutrientDefinition { get; set; } = new();
    public List<Dictionary<string, object?>> Nutrients { get; set; } = new();
}
