namespace CmcStandardPrinting.Domain.Common;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Users;

public sealed class GenericItem
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Global { get; set; }
    public int ParentCode { get; set; }
    public int CodeSite { get; set; }
    public bool Inheritable { get; set; }
    public int CodeTrans { get; set; }
    public bool IsParent { get; set; }
}

public sealed class GenericData
{
    public GenericItem Info { get; set; } = new();
    public List<GenericTranslation>? Translation { get; set; }
    public List<GenericList>? Sharing { get; set; }
    public int ActionType { get; set; }
    public List<int>? MergeList { get; set; }
    public User Profile { get; set; } = new();
    public int Classification { get; set; }
}
