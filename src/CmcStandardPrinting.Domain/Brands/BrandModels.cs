namespace CmcStandardPrinting.Domain.Brands;

public sealed class Brand
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParentCode { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public bool Flagged { get; set; }
    public string Note { get; set; } = string.Empty;
    public int Type { get; set; }
    public bool Global { get; set; }
    public int Classification { get; set; }
}

public sealed class BrandTreeNode
{
    public string Title { get; set; } = string.Empty;
    public int Key { get; set; }
    public bool Unselectable { get; set; }
    public bool Icon { get; set; }
    public List<BrandTreeNode> Children { get; set; } = new();
    public bool Select { get; set; }
    public string ParentTitle { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string AddClass { get; set; } = string.Empty;
    public int Classification { get; set; }
    public string Picture { get; set; } = string.Empty;
    public bool HasPicture { get; set; }
}

public sealed class BrandListItem
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
}
