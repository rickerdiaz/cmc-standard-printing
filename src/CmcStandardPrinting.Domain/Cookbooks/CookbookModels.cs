using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Sites;

namespace CmcStandardPrinting.Domain.Cookbooks;

public sealed class Cookbook
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParentCode { get; set; }
    public int CodeSite { get; set; }
    public int CodeOwner { get; set; }
    public bool Global { get; set; }
    public bool CanBeParent { get; set; }
    public string Picture { get; set; } = string.Empty;
    public bool HasPicture { get; set; }
}

public sealed class CookbookProfile
{
    public int Code { get; set; }
    public int CodeSite { get; set; }
}

public sealed class CookbookData
{
    public Cookbook Info { get; set; } = new();
    public CookbookProfile Profile { get; set; } = new();
    public List<GenericList> Sharing { get; set; } = new();
    public List<GenericList> Users { get; set; } = new();
}

public sealed class ResponseCookBook
{
    public ResponseCookBook(List<TreeNode> data, int total)
    {
        Data = data;
        TotalCount = total;
    }

    public List<TreeNode> Data { get; set; }
    public int TotalCount { get; set; }
}

public sealed class ResponseTreeGeneric
{
    public ResponseTreeGeneric(List<GenericTreeNode> data, int total)
    {
        Data = data;
        TotalCount = total;
    }

    public List<GenericTreeNode> Data { get; set; }
    public int TotalCount { get; set; }
}

public sealed class ResponseCookBookRecipe
{
    public ResponseCookBookRecipe(List<TreeGridNode> data, int total)
    {
        Data = data;
        TotalCount = total;
    }

    public List<TreeGridNode> Data { get; set; }
    public int TotalCount { get; set; }
}

public sealed class ProjectRecipe
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subname { get; set; } = string.Empty;
    public string PrimaryBrand { get; set; } = string.Empty;
    public string SecondaryBrand { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Image { get; set; }
    public bool Nutrition { get; set; }
    public int CheckoutUser { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
}

public sealed class GenericTreeNode
{
    public string Title { get; set; } = string.Empty;
    public int Id { get; set; }
    public List<GenericTreeNode> Children { get; set; } = new();
}

public sealed class TreeGridNode
{
    public string Title { get; set; } = string.Empty;
    public int Key { get; set; }
    public bool Expanded { get; set; }
    public bool Image { get; set; }
    public bool Leaf { get; set; }
    public string Number { get; set; } = string.Empty;
    public bool Nutrition { get; set; }
    public string Primarybrand { get; set; } = string.Empty;
    public string Secondarybrand { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Subname { get; set; } = string.Empty;
    public string ParentTitle { get; set; } = string.Empty;
    public int Note { get; set; }
    public int CheckoutUser { get; set; }
    public string Picture { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public List<TreeGridNode> Children { get; set; } = new();
}
