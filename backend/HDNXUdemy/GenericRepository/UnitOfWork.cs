using HDNXUdemyData.EntitiesContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace HDNXUdemyData.GenericRepository
{
    /// <summary>
    /// Unit of Work — wraps a single DbContext transaction across multiple repositories.
    /// Usage:
    ///   await using var tx = await _uow.BeginTransactionAsync();
    ///   await _courseRepo.AddAsync(course);
    ///   await _processRepo.AddAsync(process);
    ///   await _uow.CommitAsync();
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementation of IUnitOfWork backed by ProjectContext (EF Core).
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ProjectContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return _transaction;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction. Call BeginTransactionAsync first.");

            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
        }
    }
}
