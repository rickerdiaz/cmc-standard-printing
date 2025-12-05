using System.Collections.Generic;

namespace CmcStandardPrinting.Domain.Security;

public sealed class PasswordAndLogin
{
    public bool EnforceStrongPassword { get; set; }

    public string ExpiresAfterNumberOfDays { get; set; } = string.Empty;

    public int MinimumPasswordLength { get; set; }

    public string MinimumPasswordReuse { get; set; } = string.Empty;

    public int MaximumFailedLoginAttempts { get; set; }

    public string LockoutPeriod { get; set; } = string.Empty;
}

public sealed class PasswordAndLoginData
{
    public PasswordAndLogin Info { get; set; } = new();

    public int ActionType { get; set; }

    public List<int> MergeList { get; set; } = new();
}
