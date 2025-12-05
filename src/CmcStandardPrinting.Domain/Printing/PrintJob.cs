namespace CmcStandardPrinting.Domain.Printing;

/// <summary>
/// Represents a print job registered by the system.
/// </summary>
public record PrintJob(Guid Id, string Name, string Status, DateTime CreatedAtUtc)
{
    public static PrintJob CreateNew(string name)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new ArgumentException("Print job name cannot be empty", nameof(name));
        }

        return new PrintJob(Guid.NewGuid(), trimmedName, "Pending", DateTime.UtcNow);
    }
}
