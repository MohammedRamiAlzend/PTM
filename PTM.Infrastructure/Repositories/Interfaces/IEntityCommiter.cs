using System.Data;
using System.Security;

namespace PTM.Infrastructure.Repositories.Interfaces
{
    public interface IEntityCommiter
    {
        IPTMRepositoryBase<Project> Projects { get; }
        IPTMRepositoryBase<AppTask> Tasks { get; }
        IPTMRepositoryBase<User> Users { get; }
        IPTMRepositoryBase<Role> Roles { get; }

        IPTMRepositoryBase<T> GetRepository<T>() where T : Entity;
        int Commit();
        Task<int> CommitAsync();
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}