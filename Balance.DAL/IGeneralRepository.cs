using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Balance.DAL
{
    public interface IGeneralRepository<T, TK> where T : class
    {
        Task<bool> ExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        void Update(T item);

        Task CreateRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

        Task<T> CreateAsync(T item, CancellationToken cancellationToken = default);

        void UpdateRange(ICollection<T> items);

        Task<ICollection<T>> GetAsync<TKey>(Expression<Func<T, bool>> expression,
            Expression<Func<T, TKey>> orderByExpression = null, int skip = 0, int take = int.MaxValue,
            CancellationToken cancellationToken = default);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}