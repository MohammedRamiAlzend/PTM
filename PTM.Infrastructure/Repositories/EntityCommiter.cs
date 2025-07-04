using PTM.Infrastructure.Repositories.Interfaces;
using System;
using System.Data;
using System.Security;

namespace PTM.Infrastructure.Repositories;

public class EntityCommiter(ApplicationDbContext appDbContext, ILogger<EntityCommiter> logger) : IEntityCommiter
{
    private readonly Dictionary<Type, object> _repos = [];
    public IPTMRepositoryBase<Project> Projects => GetRepository<Project>();
    public IPTMRepositoryBase<AppTask> Tasks => GetRepository<AppTask>();
    public IPTMRepositoryBase<User> Users => GetRepository<User>();
    public IPTMRepositoryBase<Role> Roles => GetRepository<Role>();

    public int Commit()
    {
        return appDbContext.SaveChanges();
    }

    public async Task<int> CommitAsync()
    {
        return await appDbContext.SaveChangesAsync();
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogCritical($"{e.Message}");
            return 0;
        }
    }

    public void Dispose()
    {
        appDbContext.Dispose();
    }

    public IPTMRepositoryBase<T> GetRepository<T>() where T : Entity
    {
        if (_repos.ContainsKey(typeof(T)))
            return (IPTMRepositoryBase<T>)_repos[typeof(T)];
        IPTMRepositoryBase<T> newRepo = new PTMRepositoryBase<T>(appDbContext.Set<T>(), logger);
        _repos.Add(typeof(T), newRepo);
        return newRepo;
    }
}