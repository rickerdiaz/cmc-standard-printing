using CmcStandardPrinting.Application.Printing;
using CmcStandardPrinting.Domain.Printing;

namespace CmcStandardPrinting.Infrastructure.Printing;

public class InMemoryPrintJobRepository : IPrintJobRepository
{
    private readonly List<PrintJob> _jobs = new();

    public Task<PrintJob> AddAsync(PrintJob job, CancellationToken cancellationToken = default)
    {
        _jobs.Add(job);
        return Task.FromResult(job);
    }

    public Task<IReadOnlyCollection<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PrintJob> snapshot = _jobs.ToList();
        return Task.FromResult(snapshot);
    }
}
