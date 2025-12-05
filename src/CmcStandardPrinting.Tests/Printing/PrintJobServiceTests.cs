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

    [Fact]
    public async Task UpdateStatusAsync_ReturnsNullWhenMissing()
    {
        var repository = new FakePrintJobRepository();
        var service = new PrintJobService(repository);

        var result = await service.UpdateStatusAsync(Guid.NewGuid(), "Processing");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_PersistsChange()
    {
        var repository = new FakePrintJobRepository();
        var job = await repository.AddAsync(PrintJob.CreateNew("Labels"));
        var service = new PrintJobService(repository);

        var updated = await service.UpdateStatusAsync(job.Id, "Completed");

        Assert.NotNull(updated);
        Assert.Equal("Completed", updated!.Status);
        var persisted = await repository.GetByIdAsync(job.Id);
        Assert.Equal("Completed", persisted!.Status);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingJob()
    {
        var repository = new FakePrintJobRepository();
        var job = await repository.AddAsync(PrintJob.CreateNew("Labels"));
        var service = new PrintJobService(repository);

        var deleted = await service.DeleteAsync(job.Id);

        Assert.True(deleted);
        Assert.Empty(repository.StoredJobs);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseWhenNotFound()
    {
        var repository = new FakePrintJobRepository();
        var service = new PrintJobService(repository);

        var deleted = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(deleted);
    }

    private class FakePrintJobRepository : IPrintJobRepository
    {
        public List<PrintJob> StoredJobs { get; } = new();

        public Task<PrintJob> AddAsync(PrintJob job, CancellationToken cancellationToken = default)
        {
            StoredJobs.Add(job);
            return Task.FromResult(job);
        }

        public Task<PrintJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PrintJob?>(StoredJobs.SingleOrDefault(j => j.Id == id));
        }

        public Task<IReadOnlyCollection<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<PrintJob>>(StoredJobs.ToList());
        }

        public Task<PrintJob?> UpdateAsync(PrintJob job, CancellationToken cancellationToken = default)
        {
            var existing = StoredJobs.FindIndex(j => j.Id == job.Id);
            if (existing == -1)
            {
                return Task.FromResult<PrintJob?>(null);
            }

            StoredJobs[existing] = job;
            return Task.FromResult<PrintJob?>(job);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var removed = StoredJobs.RemoveAll(j => j.Id == id) > 0;
            return Task.FromResult(removed);
        }
    }
}
