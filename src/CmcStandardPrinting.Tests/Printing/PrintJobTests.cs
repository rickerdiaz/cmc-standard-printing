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
}
