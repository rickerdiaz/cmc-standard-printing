namespace CmcStandardPrinting.Domain.Kiosks;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Sources;
using CmcStandardPrinting.Domain.Users;

public sealed class Kiosk
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Global { get; set; }
    public int Type { get; set; }
}

public sealed class RecipeBrandSite
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public string DateFrom { get; set; } = string.Empty;
    public string DateTo { get; set; } = string.Empty;
    public int HitCounter { get; set; }
}

public sealed class KioskData
{
    public User Profile { get; set; } = new();
    public Source Info { get; set; } = new();
    public List<GenericList> Sharing { get; set; } = new();
    public List<GenericList> Keywords { get; set; } = new();
    public int ActionType { get; set; }
    public List<int> MergeList { get; set; } = new();
}

public sealed class KioskSearch
{
    public int CodeSite { get; set; }
    public int CodeTrans { get; set; }
    public string Name { get; set; } = string.Empty;
}
