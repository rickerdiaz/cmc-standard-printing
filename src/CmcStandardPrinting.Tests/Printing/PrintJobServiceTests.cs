using CmcStandardPrinting.Application.Printing;
using CmcStandardPrinting.Domain.Printing;
using Xunit;

namespace CmcStandardPrinting.Tests.Printing;

public class PrintJobServiceTests
{
    [Fact]
    public async Task CreateAsync_StoresTrimmedJob()
    {
        var repository = new FakePrintJobRepository();
        var service = new PrintJobService(repository);

        var job = await service.CreateAsync("   Labels  ");

        Assert.Equal("Labels", job.Name);
        Assert.Single(repository.StoredJobs);
        Assert.Equal(job, repository.StoredJobs.Single());
    }

    [Fact]
    public async Task GetAsync_ReturnsRepositorySnapshot()
    {
        var repository = new FakePrintJobRepository();
        var existing = PrintJob.CreateNew("Existing Job");
        await repository.AddAsync(existing);
        var service = new PrintJobService(repository);

        var jobs = await service.GetAsync();

        Assert.Single(jobs);
        Assert.Equal(existing, jobs.Single());
    }

    private class FakePrintJobRepository : IPrintJobRepository
    {
        public List<PrintJob> StoredJobs { get; } = new();

        public Task<PrintJob> AddAsync(PrintJob job, CancellationToken cancellationToken = default)
        {
            StoredJobs.Add(job);
            return Task.FromResult(job);
        }

        public Task<IReadOnlyCollection<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<PrintJob>>(StoredJobs.ToList());
        }
    }
}
