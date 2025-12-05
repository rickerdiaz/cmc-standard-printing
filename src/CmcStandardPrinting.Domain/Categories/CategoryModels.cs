namespace CmcStandardPrinting.Domain.Categories;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;

public sealed class Category
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Global { get; set; }
    public int Type { get; set; }
    public string Picture { get; set; } = string.Empty;
    public int Archive { get; set; }
}

public sealed class CategoryTranslation
{
    public string Name { get; set; } = string.Empty;
    public string Name2 { get; set; } = string.Empty;
    public int CodeTrans { get; set; }
    public int CodeSite { get; set; }
    public string NamePlural { get; set; } = string.Empty;
}

public sealed class CategoryData
{
    public User Profile { get; set; } = new();
    public Category Info { get; set; } = new();
    public List<CategoryTranslation> Translation { get; set; } = new();
    public AutoNumber AutoNumber { get; set; } = new();
    public int ActionType { get; set; }
    public List<int> MergeList { get; set; } = new();
    public List<SharingItem> Sharing { get; set; } = new();
}

public sealed class AutoNumber
{
    public int AutoNumberCodeSite { get; set; }
    public bool AutoNumberFlag { get; set; }
    public string AutoNumberPrefix { get; set; } = string.Empty;
    public string AutoNumberStart { get; set; } = string.Empty;
    public bool AutoNumberKeepPrefixLength { get; set; }
}

public sealed class RecipeTranslation
{
    public int CodeTrans { get; set; }
    public string TranslationName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public int HasPicture { get; set; }
    public int Archive { get; set; }
}

public sealed class GenericCodeValueList
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
}
