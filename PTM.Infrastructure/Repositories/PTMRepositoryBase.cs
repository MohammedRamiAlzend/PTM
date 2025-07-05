using PTM.Domain.Entities.Interfaces;

namespace PTM.Infrastructure.Repositories;

public class PTMRepositoryBase<TEntity>(DbSet<TEntity> dbSet, ILogger logger) : IPTMRepositoryBase<TEntity>
    where TEntity :  Entity
{
    public async Task<DbRequest> AddAsync(TEntity entity)
    {
        if (entity == null) return DbRequest.Failure("Entity cannot be null.");

        return await ExecuteOperationAsync(
            async () =>
            {
                await dbSet.AddAsync(entity);
                return DbRequest.Success();
            },
            $"Entity of type {typeof(TEntity).Name} has been added successfully",
            $"Failed to add entity of type {typeof(TEntity).Name}."
        );
    }

    public async Task<DbRequest<List<TEntity>>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = dbSet;

        try
        {
            if (filter != null) query = query.Where(filter);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);

            var result = await query.ToListAsync();
            return result.Count == 0
                ? DbRequest<List<TEntity>>.Success($"No entities of type {typeof(TEntity).Name} found.")
                : DbRequest<List<TEntity>>.Success(result, "Entities retrieved successfully.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving entities of type {EntityType}.", typeof(TEntity).Name);
            return DbRequest<List<TEntity>>.Failure(
                $"Something went wrong while retrieving entities of type {typeof(TEntity).Name}. Exception: {e.Message}");
        }
    }

    public async Task<PaginatedDbRequest<TEntity>> GetAllPaginatedAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return PaginatedDbRequest<TEntity>.Failure("Page number and size must be greater than zero.");

        IQueryable<TEntity> query = dbSet;

        try
        {
            if (filter != null) query = query.Where(filter);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items.Count == 0) return PaginatedDbRequest<TEntity>.Failure($"No entities of type {typeof(TEntity).Name} found.");

            return PaginatedDbRequest<TEntity>.Success(
                items,
                totalCount,
                pageNumber,
                pageSize,
                "Entities retrieved successfully.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving paginated entities of type {EntityType}.", typeof(TEntity).Name);
            return PaginatedDbRequest<TEntity>.Failure(
                $"Something went wrong while retrieving paginated entities of type {typeof(TEntity).Name}. Exception: {e.Message}");
        }
    }

    public async Task<DbRequest<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = dbSet;

        try
        {
            if (filter != null) query = query.Where(filter);
            if (include != null) query = include(query);

            var entity = await query.FirstOrDefaultAsync();
            return entity == null
                ? DbRequest<TEntity>.Failure($"Entity of type {typeof(TEntity).Name} was not found.")
                : DbRequest<TEntity>.Success(entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving entity of type {EntityType}.", typeof(TEntity).Name);
            return DbRequest<TEntity>.Failure(
                $"Something went wrong while retrieving an entity of type {typeof(TEntity).Name}. Exception: {e.Message}");
        }
    }

    public async Task<DbRequest> RemoveAsync(Expression<Func<TEntity, bool>> filter)
    {
        if (filter == null) return DbRequest.Failure("Filter cannot be null.");

        var getEntity = await GetAsync(filter);
        if (!getEntity.IsSuccess || getEntity.Data == null) return DbRequest.Failure("Entity not found.");

        return await ExecuteOperationAsync(
            async () =>
            {
                var entity = getEntity.Data;
                if (entity is ISoftDeletable softDeletable)
                {
                    softDeletable.IsDeleted = true;
                    dbSet.Update(entity); 
                }
                else
                {
                    dbSet.Remove(entity);
                }
                return DbRequest.Success();
            },
            $"Entity of type {typeof(TEntity).Name} with ID {getEntity.Data.Id} has been deleted.",
            $"Failed to delete entity of type {typeof(TEntity).Name}."
        );
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (include != null) query = include(query);
        return query.AnyAsync(filter);
    }
    public async Task<DbRequest> UpdateAsync(TEntity entity)
    {
        if (entity == null) return DbRequest.Failure("Entity cannot be null.");
        if (entity.Id == 0) return DbRequest.Failure("Entity key (Id) cannot be zero or null.");

        return await ExecuteOperationAsync(
            async () =>
            {
                dbSet.Attach(entity);
                dbSet.Entry(entity).State = EntityState.Modified;
                return DbRequest.Success();
            },
            $"Entity of type {typeof(TEntity).Name} has been updated successfully.",
            $"Failed to update entity of type {typeof(TEntity).Name}."
        );
    }
    
    private async Task<DbRequest> ExecuteOperationAsync(Func<Task<DbRequest>> operation,
        string successMessage = "",
        string errorMessage = "")
    {
        try
        {
            var result = await operation();
            result.Message = result.IsSuccess ? successMessage : errorMessage;
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during operation for entity of type {EntityType}.", typeof(TEntity).Name);
            return DbRequest.Failure($"An error occurred during the operation. Exception: {e.Message}");
        }
    }
}
