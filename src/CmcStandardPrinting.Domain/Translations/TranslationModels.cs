namespace CmcStandardPrinting.Domain.Translations;

public sealed class RecipeIngredientTranslation
{
    public int ItemId { get; set; }
    public int CodeTrans { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Complement { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public string AlternativeIngredient { get; set; } = string.Empty;
    public int CodeUnitDisplaySelection { get; set; }
    public bool IsGenderSensitive { get; set; }
    public int PrefixCode { get; set; }
    public string PrefixGender { get; set; } = string.Empty;
}

public sealed class RecipeProcedureTranslation
{
    public int NoteId { get; set; }
    public int CodeTrans { get; set; }
    public string Note { get; set; } = string.Empty;
    public string AbbrevNote { get; set; } = string.Empty;
}
