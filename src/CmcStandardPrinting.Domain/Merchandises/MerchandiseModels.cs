namespace CmcStandardPrinting.Domain.Merchandises;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Nutrients;
using CmcStandardPrinting.Domain.Printers;

public sealed class Merchandise
{
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public int CodeListe { get; set; }
    public int CodeSite { get; set; }
    public int CodeUser { get; set; }
    public string UPC { get; set; } = string.Empty;
    public int CodeBrand { get; set; }
    public string Brand { get; set; } = string.Empty;
    public int CodeCategory { get; set; }
    public string Category { get; set; } = string.Empty;
    public int CodeSupplier { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public int CodeTrans { get; set; }
    public int DefaultPicture { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CodeModifiedBy { get; set; }
    public List<string>? CustomTempPictures { get; set; }
    public List<MerchandiseAttachment>? CustomTempAttachments { get; set; }
    public string Date { get; set; } = string.Empty;
    public string ModifiedDate { get; set; } = string.Empty;
    public int Wastage1 { get; set; }
    public int Wastage2 { get; set; }
    public int Wastage3 { get; set; }
    public int Wastage4 { get; set; }
    public int Wastage5 { get; set; }
    public List<string>? Picture { get; set; }
    public bool InUse { get; set; }
    public int CodeSetPrice { get; set; }
    public string CodeLink { get; set; } = string.Empty;
    public bool cGlobal { get; set; }
    public bool AllergenApproved { get; set; }
    public string LinkNutrient { get; set; } = string.Empty;
    public int CodeNutrientSet { get; set; }
    public int CodeCountry { get; set; }
    public string Country { get; set; } = string.Empty;
}

public sealed class MerchandiseAttachment
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class MerchandisePrice
{
    public string History { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int CodeUnit { get; set; }
    public double Price { get; set; }
    public double Ratio { get; set; }
    public int TaxCode { get; set; }
    public double TaxValue { get; set; }
    public int Position { get; set; }
    public int CodeSetPrice { get; set; }
    public bool IsUsed { get; set; }
}

public sealed class MerchandiseTranslation
{
    public int Id { get; set; }
    public int TranslationCode { get; set; }
    public string TranslationName { get; set; } = string.Empty;
    public int CodeDictionary { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public string CookingTip { get; set; } = string.Empty;
    public string Refinement { get; set; } = string.Empty;
    public string SpecificDetermination { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;
    public string Productivity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PrefixCode { get; set; } = string.Empty;
    public string PrefixName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public bool IsGenderSensitive { get; set; }
}

public sealed class MerchandiseHistory
{
    public string DateAudit { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string FieldCode { get; set; } = string.Empty;
    public string Previous { get; set; } = string.Empty;
    public string HNew { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string AuditType { get; set; } = string.Empty;
    public string CodeListe { get; set; } = string.Empty;
    public string CodeUser { get; set; } = string.Empty;
    public string IsCode { get; set; } = string.Empty;
}

public sealed class MerchandiseData
{
    public Merchandise Info { get; set; } = new();
    public List<MerchandiseHistory>? History { get; set; }
    public List<MerchandisePrice>? Price { get; set; }
    public List<GenericTree>? Keywords { get; set; }
    public List<RecipeNutrition>? Nutrient { get; set; }
    public List<MerchandiseAttachment>? Attachment { get; set; }
    public List<MerchandiseTranslation>? Translation { get; set; }
    public List<TreeNode>? Sharing { get; set; }
    public List<ListeAllergen>? Allergen { get; set; }
    public bool AllergenUpdated { get; set; }
    public bool hasApprover { get; set; }
    public bool submitted { get; set; }
    public int NextRoleLevelApprover { get; set; }
    public int TaxCode { get; set; }
}

public sealed class RecipeHistory
{
    public string DateAudit { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string FieldCode { get; set; } = string.Empty;
    public string Previous { get; set; } = string.Empty;
    public string HNew { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string AuditType { get; set; } = string.Empty;
    public string CodeListe { get; set; } = string.Empty;
    public string CodeUser { get; set; } = string.Empty;
    public string IsCode { get; set; } = string.Empty;
}

public sealed class RecipeHistoryResponse
{
    public RecipeHistoryResponse() { }

    public RecipeHistoryResponse(List<RecipeHistory> histories, int totalCount)
    {
        Histories = histories;
        TotalCount = totalCount;
    }

    public List<RecipeHistory> Histories { get; set; } = new();
    public int TotalCount { get; set; }
}
