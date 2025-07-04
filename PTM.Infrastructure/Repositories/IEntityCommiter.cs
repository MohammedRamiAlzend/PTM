using System.Data;
using System.Security;

namespace PTM.Infrastructure.Repositories
{
    public interface IEntityCommiter
    {
        IPTMRepositoryBase<Project> Projects { get; }
        IPTMRepositoryBase<AppTask> Tasks { get; }

        IPTMRepositoryBase<T> GetRepository<T>() where T : Entity;
        int Commit();
        Task<int> CommitAsync();
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}