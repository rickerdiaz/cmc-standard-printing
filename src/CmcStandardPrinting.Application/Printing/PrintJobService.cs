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

    public Task<PrintJob> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var job = PrintJob.CreateNew(name);
        return _repository.AddAsync(job, cancellationToken);
    }
}
