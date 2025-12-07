using System.Collections.Generic;
using CmcStandardPrinting.Application.Printing;
using CmcStandardPrinting.Domain.Printing;

namespace CmcStandardPrinting.Infrastructure.Printing;

public class InMemoryPrintJobRepository : IPrintJobRepository
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<Guid, PrintJob> _jobs = new();

    public Task<PrintJob> AddAsync(PrintJob job, CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _jobs[job.Id] = job;
        }
        return Task.FromResult(job);
    }

    public Task<IReadOnlyCollection<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            IReadOnlyCollection<PrintJob> snapshot = _jobs.Values.ToList();
            return Task.FromResult(snapshot);
        }
    }

    public Task<PrintJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _jobs.TryGetValue(id, out var job);
            return Task.FromResult(job);
        }
    }

    public Task<PrintJob?> UpdateAsync(PrintJob job, CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            if (!_jobs.ContainsKey(job.Id))
            {
                return Task.FromResult<PrintJob?>(null);
            }

            _jobs[job.Id] = job;
            return Task.FromResult<PrintJob?>(job);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            return Task.FromResult(_jobs.Remove(id));
        }
    }
}
