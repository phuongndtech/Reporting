namespace Reporting.Application.Common.Interfaces;

public interface IApplicationDbContext: IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    int SaveChanges();
}