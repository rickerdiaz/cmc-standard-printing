namespace CmcStandardPrinting.Domain.Recipes;

public sealed class RecipeAttachment
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
