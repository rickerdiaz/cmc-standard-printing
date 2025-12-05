using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;

namespace CmcStandardPrinting.Domain.Times;

public sealed class Time
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public bool Global { get; set; }
    public int CodeTrans { get; set; }
    public int CodeUser { get; set; }
    public int CodeSite { get; set; }
    public bool IsTotal { get; set; }
    public bool RequiredTotal { get; set; }
}

public sealed class TimeData
{
    public Time Info { get; set; } = new();
    public User Profile { get; set; } = new();
    public List<GenericTree> Sites { get; set; } = new();
    public List<GenericTranslation> Translation { get; set; } = new();
    public int ActionType { get; set; }
    public List<int> MergeList { get; set; } = new();
    public List<GenericList> Sharing { get; set; } = new();
}
