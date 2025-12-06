using CmcStandardPrinting.Domain.Printing;
using Xunit;

namespace CmcStandardPrinting.Tests.Printing;

public class PrintJobTests
{
    [Fact]
    public void CreateNew_RejectsBlankName()
    {
        Action act = () => PrintJob.CreateNew("   ");

        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void CreateNew_TrimsNameAndDefaultsStatus()
    {
        var job = PrintJob.CreateNew("  Invoice Batch  ");

        Assert.Equal("Invoice Batch", job.Name);
        Assert.Equal("Pending", job.Status);
        Assert.True(job.CreatedAtUtc <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("Processing")]
    [InlineData("completed")]
    public void WithStatus_UpdatesToAllowedStatus(string status)
    {
        var job = PrintJob.CreateNew("Labels");

        var updated = job.WithStatus(status);

        Assert.Equal(job.Id, updated.Id);
        Assert.Equal(job.Name, updated.Name);
        Assert.Contains(updated.Status, PrintJob.AllowedStatuses);
    }

    [Fact]
    public void WithStatus_RejectsInvalidStatus()
    {
        var job = PrintJob.CreateNew("Labels");

        Action act = () => job.WithStatus("Unknown");

        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("status", exception.ParamName);
    }
}
