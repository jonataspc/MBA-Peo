using EFCore.BulkExtensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;
using Peo.Core.Interfaces.Data;
using System.Linq.Expressions;

namespace Peo.Core.Infra.Data.Repositories
{
    public class GenericRepository<TEntity, TContext> : IRepository<TEntity>
       where TEntity : EntityBase, IAggregateRoot
       where TContext : DbContext
    {
        // Default behavior: tracking is disabled
        protected bool _isTracking = false;

        public IUnitOfWork UnitOfWork => (IUnitOfWork)_dbContext;

        protected readonly TContext _dbContext;

        public GenericRepository(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<TEntity> WithTracking()
        {
            _isTracking = true;
            return this;
        }

        public IRepository<TEntity> WithoutTracking()
        {
            _isTracking = false;
            return this;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbContext.AddRangeAsync(entities).ConfigureAwait(false);
        }

        public virtual async Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            await _dbContext.BulkInsertAsync(entities.ToList(), bulkConfig =>
            {
                const int bulkBatchSize = 500;
                bulkConfig.BatchSize = bulkBatchSize;
            })
            .ConfigureAwait(false);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entityToRemove = await _dbContext.Set<TEntity>().SingleOrDefaultAsync(g => g.Id == id).ConfigureAwait(false);

            if (entityToRemove != null)
            {
                _dbContext.Set<TEntity>().Remove(entityToRemove);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var list = await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
            _dbContext.Set<TEntity>().RemoveRange(list);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<TEntity?> GetAsync(Guid id)
        {
            var dbSet = _dbContext.Set<TEntity>();

            if (!_isTracking)
            {
                return await dbSet.AsNoTracking()
                                  .Where(x => x.Id == id)
                                  .FirstOrDefaultAsync();
            }
            else
            {
                return await dbSet.FindAsync(id).ConfigureAwait(false);
            }
        }

        public virtual async Task<IEnumerable<TEntity>?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TSelect>?> GetAsync<TSelect>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TSelect>> selector)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.Select(selector).ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>?> GetAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, Expression<Func<TEntity, object>>? orderBy = null, Expression<Func<TEntity, object>>? orderByDescending = null)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (orderBy is not null)
            {
                query = query.OrderBy(orderBy);
            }

            if (orderByDescending is not null)
            {
                query = query.OrderByDescending(orderByDescending);
            }

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>?> GetAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, IEnumerable<Expression<Func<TEntity, object>>> orderByExpressions, IEnumerable<Expression<Func<TEntity, object>>> orderByDescendingExpressions)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            IOrderedQueryable<TEntity> orderedQuery = query.OrderBy(o => 0);

            foreach (var orderBy in orderByExpressions)
            {
                orderedQuery = orderedQuery.ThenBy(orderBy);
            }

            foreach (var orderBy in orderByDescendingExpressions)
            {
                orderedQuery = orderedQuery.ThenByDescending(orderBy);
            }

            query = orderedQuery;

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TOut>?> GetProjectedAsync<TOut>(Expression<Func<TEntity, bool>> predicate)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ProjectToType<TOut>().ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TResult>> GetGroupedAsync<TKey, TResult>(Expression<Func<TEntity, TKey>> groupingKey, Expression<Func<IGrouping<TKey, TEntity>, TResult>> resultSelector, Expression<Func<TEntity, bool>>? filter = null)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.GroupBy(groupingKey)
                              .Select(resultSelector)
                              .ToListAsync()
                              .ConfigureAwait(false);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(TEntity entity, params Expression<Func<TEntity, object?>>[] propertiesToUpdate)
        {
            foreach (var property in propertiesToUpdate)
            {
                _dbContext.Entry(entity).Property(property).IsModified = true;
            }
        }

        public async Task<int> BulkDeleteAsync(Expression<Func<TEntity, bool>> query)
        {
            return await _dbContext.Set<TEntity>()
                .Where(query)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
        }

        public async Task<int> BulkUpdateAsync(Expression<Func<TEntity, bool>> query, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expression)
        {
            return await _dbContext.Set<TEntity>()
                .Where(query)
                .ExecuteUpdateAsync(expression)
                .ConfigureAwait(false);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>()
                .Where(predicate)
                .CountAsync()
                .ConfigureAwait(false);
        }

        public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>()
                .Where(predicate)
                .LongCountAsync()
                .ConfigureAwait(false);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>? orderBy = null, Expression<Func<TEntity, object>>? orderByDescending = null)
        {
            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (!_isTracking)
            {
                query = query.AsNoTracking();
            }

            if (orderBy is not null)
            {
                query = query.OrderBy(orderBy);
            }

            if (orderByDescending is not null)
            {
                query = query.OrderByDescending(orderByDescending);
            }

            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .AnyAsync(predicate)
                                   .ConfigureAwait(false);
        }
    }
}