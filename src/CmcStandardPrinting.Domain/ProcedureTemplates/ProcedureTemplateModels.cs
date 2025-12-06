using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Recipes;
using CmcStandardPrinting.Domain.Translations;

namespace CmcStandardPrinting.Domain.ProcedureTemplates;

public sealed class ProcedureTemplate
{
    public ProcedureTemplateInfo? Info { get; set; }
    public List<RecipeTranslation>? Translations { get; set; }
    public List<RecipeProcedure>? Procedures { get; set; }
    public List<GenericList>? Sharing { get; set; }
    public List<GenericList>? Users { get; set; }
    public bool Global { get; set; }
    public string? TempPictures { get; set; }
}

public sealed class ProcedureTemplateInfo
{
    public int CodeUser { get; set; }
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Global { get; set; }
    public int Type { get; set; }
    public int CodeTrans { get; set; }
    public int CodeSite { get; set; }
    public string Note { get; set; } = string.Empty;
}
