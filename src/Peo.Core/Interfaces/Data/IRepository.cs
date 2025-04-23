using Peo.Core.DomainObjects;
using System.Linq.Expressions;

namespace Peo.Core.Interfaces.Data
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetAsync(Guid id);

        Task<IEnumerable<TSelect>?> GetAsync<TSelect>(Expression<Func<T, bool>> predicate, Expression<Func<T, TSelect>> selector);

        Task<IEnumerable<T>?> GetAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>?> GetAsync(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>>? orderBy = null, Expression<Func<T, object>>? orderByDescending = null);

        Task<IEnumerable<T>?> GetAsync(Expression<Func<T, bool>> predicate, int skip, int take, IEnumerable<Expression<Func<T, object>>> orderByExpressions, IEnumerable<Expression<Func<T, object>>> orderByDescendingExpressions);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? orderBy = null, Expression<Func<T, object>>? orderByDescending = null);

        void Insert(T entity);

        Task BulkInsertAsync(IEnumerable<T> entities);

        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Update(T entity, params Expression<Func<T, object?>>[] propertiesToUpdate);

        Task DeleteAsync(Guid id);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        Task<long> LongCountAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<TResult>> GetGroupedAsync<TKey, TResult>(Expression<Func<T, TKey>> groupingKey, Expression<Func<IGrouping<TKey, T>, TResult>> resultSelector, Expression<Func<T, bool>>? filter = null);

        Task<int> BulkDeleteAsync(Expression<Func<T, bool>> query);

        IRepository<T> WithTracking();

        IRepository<T> WithoutTracking();

        Task<IEnumerable<TOut>?> GetProjectedAsync<TOut>(Expression<Func<T, bool>> predicate);
    }
}