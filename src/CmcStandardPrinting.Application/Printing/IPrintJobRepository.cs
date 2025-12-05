using CmcStandardPrinting.Domain.Printing;

namespace CmcStandardPrinting.Application.Printing;

public interface IPrintJobRepository
{
    Task<IReadOnlyCollection<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PrintJob> AddAsync(PrintJob job, CancellationToken cancellationToken = default);
}
