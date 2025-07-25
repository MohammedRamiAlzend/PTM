﻿namespace PTM.Infrastructure.Repositories;

public interface IPTMRepositoryBase<TEntity> where TEntity : Entity
{
    Task<DbRequest<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    Task<DbRequest<List<TEntity>>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderby = null);



    Task<DbRequest> AddAsync(TEntity entity);
    Task<DbRequest> UpdateAsync(TEntity entity);
    Task<DbRequest> RemoveAsync(Expression<Func<TEntity, bool>> filter);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
}