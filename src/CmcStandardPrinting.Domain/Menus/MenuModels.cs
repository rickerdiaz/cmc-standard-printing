namespace CmcStandardPrinting.Domain.Menus;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Allergens;
using CmcStandardPrinting.Domain.ProcedureTemplates;

public sealed class MenuData
{
    public Menu? Info { get; set; }
    public List<MenuAttachment>? Attachment { get; set; }
    public List<MenuCalculation>? Calculation { get; set; }
    public List<MenuNutrition>? Nutrition { get; set; }
    public List<MenuIngredient>? Ingredient { get; set; }
    public List<MenuProcedure>? Procedure { get; set; }
    public List<MenuTranslation>? Translation { get; set; }
    public List<MenuComment>? Comment { get; set; }
    public List<ListeAllergen>? Allergen { get; set; }
    public List<ProcedureTemplateInfo>? ProcedureTemplate { get; set; }
}

public sealed class Menu
{
    public int Type { get; set; }
    public int CodeListe { get; set; }
    public string? Number { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public int CodeCategory { get; set; }
    public string? Remark { get; set; }
    public double Yield { get; set; }
    public string? YieldUnit { get; set; }
    public int CodeYieldUnit { get; set; }
    public int CodeTrans { get; set; }
    public string? Date1 { get; set; }
    public string? Description { get; set; }
    public double SrWeight { get; set; }
    public double SrQty { get; set; }
    public string? SrUnit { get; set; }
    public int SrUnitCode { get; set; }
    public int CodeSite { get; set; }
    public int CodeUser { get; set; }
    public string? DateCreated { get; set; }
    public string? DateLastModified { get; set; }
    public string? CreatedBy { get; set; }
    public int CodeCreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public int CodeModifiedBy { get; set; }
    public string? Pictures { get; set; }
    public int DefaultPicture { get; set; }
    public string? Note { get; set; }
    public string? FootNote1 { get; set; }
    public string? FootNote2 { get; set; }
    public string? FootNote1Clean { get; set; }
    public string? FootNote2Clean { get; set; }
    public string? MethodFormat { get; set; }
    public bool IsGlobal { get; set; }
    public string? Source { get; set; }
    public int CodeSource { get; set; }
}

public sealed class MenuAttachment
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Resource { get; set; }
    public int Type { get; set; }
    public bool IsDefault { get; set; }
}

public sealed class MenuCalculation
{
    public int Id { get; set; }
    public int CodeListe { get; set; }
    public double Coef { get; set; }
    public double CalcPrice { get; set; }
    public double ImposedPrice { get; set; }
    public int CodeSetPrice { get; set; }
    public int Tax { get; set; }
    public double TaxValue { get; set; }
}

public sealed class MenuNutrition
{
    public int Id { get; set; }
    public int Nutr_No { get; set; }
    public int Position { get; set; }
    public string? Name { get; set; }
    public string? TagName { get; set; }
    public string? Format { get; set; }
    public double Value { get; set; }
    public double Imposed { get; set; }
    public double Percent { get; set; }
    public string? Unit { get; set; }
    public int GDA { get; set; }
    public int CodeNutrientSet { get; set; }
    public string? NutrientSet { get; set; }
    public bool DisplayNutrition { get; set; }
    public bool Display { get; set; }
    public int ImposedType { get; set; }
    public string? PortionSize { get; set; }
    public string? NutritionBasis { get; set; }
}

public sealed class MenuIngredientTranslation
{
    public int ItemId { get; set; }
    public int CodeTrans { get; set; }
    public string? Name { get; set; }
    public string? Remark { get; set; }
    public string? Complement { get; set; }
    public string? Note { get; set; }
    public string? Preparation { get; set; }
    public string? AlternativeIngredient { get; set; }
    public int Step { get; set; }
}

public sealed class MenuIngredient
{
    public int CodeListe { get; set; }
    public int CodeUser { get; set; }
    public int ItemId { get; set; }
    public int ItemCode { get; set; }
    public string? ItemName { get; set; }
    public int ItemType { get; set; }
    public double ItemQty { get; set; }
    public int ItemCodeUnit { get; set; }
    public string? ItemUnit { get; set; }
    public int Step { get; set; }
    public int Position { get; set; }
    public double itemSellingPrice { get; set; }
    public double Cons { get; set; }
    public double ImposedPrice { get; set; }
    public string? Complement { get; set; }
    public string? Preparation { get; set; }
    public string? AlternativeIngredient { get; set; }
    public string? TmpName { get; set; }
    public string? TmpQty { get; set; }
    public string? TmpUnit { get; set; }
    public string? TmpComplement { get; set; }
    public string? TmpPreparation { get; set; }
    public int Wastage1 { get; set; }
    public int Wastage2 { get; set; }
    public int Wastage3 { get; set; }
    public int Wastage4 { get; set; }
    public double Price { get; set; }
    public double YieldIng { get; set; }
    public string? PriceUnit { get; set; }
    public double Amount { get; set; }
    public double QuantityMetric { get; set; }
    public int CodeUnitMetric { get; set; }
    public string? UnitMetric { get; set; }
    public double QuantityImperial { get; set; }
    public int CodeUnitImperial { get; set; }
    public string? UnitImperial { get; set; }
    public int ConvertDirection { get; set; }
    public bool IsQuickEncode { get; set; }
    public bool IsAllowMetricImperial { get; set; }
    public int ApprovalStatusCode { get; set; }
    public int ApprovalRequestedBy { get; set; }
    public string? ApprovalRequestedDate { get; set; }
    public int ApprovalBy { get; set; }
    public string? ApprovalDate { get; set; }
    public int CodeBrand { get; set; }
    public int CodeUnitDisplaySelection { get; set; }
    public bool isLocked { get; set; }
    public string? Remark { get; set; }
    public double Factor { get; set; }
    public List<MenuIngredientTranslation>? Translation { get; set; }
}

public sealed class MenuProcedureTranslation
{
    public int NoteId { get; set; }
    public int CodeTrans { get; set; }
    public string? Note { get; set; }
    public string? AbbrevNote { get; set; }
}

public sealed class MenuProcedure
{
    public int NoteId { get; set; }
    public int Position { get; set; }
    public string? Note { get; set; }
    public string? AbbrevNote { get; set; }
    public List<MenuProcedureTranslation>? Translation { get; set; }
    public string? Picture { get; set; }
    public string? hasPicture { get; set; }
}

public sealed class MenuTranslation
{
    public int Id { get; set; }
    public int CodeTrans { get; set; }
    public string? TranslationName { get; set; }
    public string? Name { get; set; }
    public string? Remark { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

public sealed class MenuComment
{
    public int Sequence { get; set; }
    public int Owner { get; set; }
    public string? Description { get; set; }
    public string? PostedBy { get; set; }
    public string? SubmitDate { get; set; }
    public string? DateLastModified { get; set; }
}
