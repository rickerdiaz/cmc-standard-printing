using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;

namespace CmcStandardPrinting.Domain.Suppliers;

public sealed class Supplier
{
    public int Code { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public bool Global { get; set; }
    public string City { get; set; } = string.Empty;
    public int CodeUser { get; set; }
    public int CodeSite { get; set; }
    public int ActionType { get; set; }
}

public sealed class SupplierData
{
    public User Profile { get; set; } = new();
    public Supplier Info { get; set; } = new();
    public List<GenericList> Sharing { get; set; } = new();
    public int ActionType { get; set; }
    public List<int> MergeList { get; set; } = new();
}
