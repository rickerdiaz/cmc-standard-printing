namespace CmcStandardPrinting.Domain.Aliases;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;

public sealed class AliasInfo
{
    public int Code { get; set; }
    public int IdMain { get; set; }
    public int CodeTrans { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
}

public sealed class AliasData
{
    public User Profile { get; set; } = new();
    public AliasInfo Info { get; set; } = new();
    public int ActionType { get; set; }
    public List<int> MergeList { get; set; } = new();
}

public sealed class AliasRecord
{
    public int Code { get; set; }
    public int IdMain { get; set; }
    public int CodeTrans { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
}
