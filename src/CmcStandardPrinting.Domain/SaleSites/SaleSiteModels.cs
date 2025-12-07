namespace CmcStandardPrinting.Domain.SaleSites;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;

public sealed class SaleSite
{
    public int Code { get; set; }
    public string LocationNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CertificationID { get; set; } = string.Empty;
    public string IsProductionLocation { get; set; } = string.Empty;
    public string IsSalesSite { get; set; } = string.Empty;
    public string CodeLanguage { get; set; } = string.Empty;
}

public sealed class SaleSiteData
{
    public User Profile { get; set; } = new();
    public SaleSite Info { get; set; } = new();
    public List<GenericList> Sharing { get; set; } = new();
    public int ActionType { get; set; }
    public List<int> MergeList { get; set; } = new();
    public List<GenericTranslation> Translation { get; set; } = new();
}

public sealed class User
{
    public int Code { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CodeSite { get; set; }
    public int RoleLevel { get; set; }
    public int SalesSite { get; set; }
    public int SalesSiteLanguage { get; set; }
    public int SalesSiteRole { get; set; }
}
