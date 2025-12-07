namespace CmcStandardPrinting.Domain.Users;

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
    public string SiteName { get; set; } = string.Empty;
}

public sealed class UserRights
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoleLevel { get; set; }
    public int Modules { get; set; }
    public int Rights { get; set; }
}

public sealed class UserData
{
    public string? Info { get; set; }
    public string? Config { get; set; }
    public string? Rights { get; set; }
}
