namespace PTM.Infrastructure.Repositories;

public interface IDbContextRepository<TEntity> where TEntity : class
{
    Task<DbRequest<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    Task<DbRequest<List<TEntity>>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderby = null);

    Task<PaginatedDbRequest<TEntity>> GetAllPaginatedAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int pageNumber = 1,
        int pageSize = 10);

    Task<DbRequest> AddAsync(TEntity entity);
    Task<DbRequest> UpdateAsync(TEntity entity);
    Task<DbRequest> RemoveAsync(Expression<Func<TEntity, bool>> filter);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

}