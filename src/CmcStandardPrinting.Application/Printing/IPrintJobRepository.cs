using CmcStandardPrinting.Domain.Printing;

namespace CmcStandardPrinting.Application.Printing;

public interface IPrintJobRepository
{
    Task<IReadOnlyCollection<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PrintJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PrintJob> AddAsync(PrintJob job, CancellationToken cancellationToken = default);
    Task<PrintJob?> UpdateAsync(PrintJob job, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
