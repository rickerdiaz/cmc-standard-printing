using System.Collections.Generic;
using System.Linq;

namespace CmcStandardPrinting.Domain.Printing;

/// <summary>
/// Represents a print job registered by the system.
/// </summary>
public record PrintJob(Guid Id, string Name, string Status, DateTime CreatedAtUtc)
{
    public static IReadOnlyList<string> AllowedStatuses { get; } = new[]
    {
        "Pending",
        "Processing",
        "Completed",
        "Failed"
    };

    public static PrintJob CreateNew(string name)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new ArgumentException("Print job name cannot be empty", nameof(name));
        }

        return new PrintJob(Guid.NewGuid(), trimmedName, "Pending", DateTime.UtcNow);
    }

    public PrintJob WithStatus(string status)
    {
        ArgumentNullException.ThrowIfNull(status);

        var trimmedStatus = status.Trim();
        if (trimmedStatus.Length == 0)
        {
            throw new ArgumentException("Status cannot be empty", nameof(status));
        }

        var normalizedStatus = AllowedStatuses
            .FirstOrDefault(s => string.Equals(s, trimmedStatus, StringComparison.OrdinalIgnoreCase));

        if (normalizedStatus is null)
        {
            throw new ArgumentException($"Status must be one of: {string.Join(", ", AllowedStatuses)}", nameof(status));
        }

        return this with { Status = normalizedStatus };
    }
}
