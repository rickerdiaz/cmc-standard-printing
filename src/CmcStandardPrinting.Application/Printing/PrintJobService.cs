using CmcStandardPrinting.Domain.Printing;

namespace CmcStandardPrinting.Application.Printing;

public class PrintJobService
{
    private readonly IPrintJobRepository _repository;

    public PrintJobService(IPrintJobRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<PrintJob>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public Task<PrintJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<PrintJob> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var job = PrintJob.CreateNew(name);
        return _repository.AddAsync(job, cancellationToken);
    }

    public async Task<PrintJob?> UpdateStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = existing.WithStatus(status);
        return await _repository.UpdateAsync(updated, cancellationToken);
    }
}
