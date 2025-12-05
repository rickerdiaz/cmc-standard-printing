namespace CmcStandardPrinting.Domain.Allergens;

public sealed class BrandTreeNode
{
    public string Title { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public bool HasPicture { get; set; }
    public string Picture { get; set; } = string.Empty;
}

public sealed class GenericCodeValueList
{
    public object? Code { get; set; }
    public object? Value { get; set; }
}

public sealed class ListeAllergen
{
    public object? CodeListe { get; set; }
    public object? CodeAllergen { get; set; }
    public object? Contain { get; set; }
    public object? Trace { get; set; }
    public object? NonAllergen { get; set; }
    public object? Derived { get; set; }
    public object? Hidden { get; set; }
}

public sealed class IngredientAllergen
{
    public object? CodeListe { get; set; }
    public object? CodeAllergen { get; set; }
    public object? Contain { get; set; }
    public object? Trace { get; set; }
    public object? NonAllergen { get; set; }
}
