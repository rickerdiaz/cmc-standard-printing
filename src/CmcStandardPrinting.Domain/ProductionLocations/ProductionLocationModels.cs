namespace CmcStandardPrinting.Domain.ProductionLocations;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;

public sealed class ProductionLocation
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
}

public sealed class ProductionLocationData
{
    public User Profile { get; set; } = new();
    public ProductionLocation Info { get; set; } = new();
    public List<GenericList> Sharing { get; set; } = new();
    public int ActionType { get; set; }
    public List<int> MergeList { get; set; } = new();
}
