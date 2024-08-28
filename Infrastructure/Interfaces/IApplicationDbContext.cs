using ZqAuth;

namespace Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Credential> Credentials { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}