using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Balance.DAL.Repositories
{
    public class GeneralRepository<T, TK> : IGeneralRepository<T, TK> where T : class
    {
        private readonly BalanceDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(BalanceDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); 
        }

        public Task<bool> ExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public void Update(T item)
        {
            if (item == null) return;
            _context.Entry(item).State = EntityState.Modified;
        }

        public Task CreateRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            return _dbSet.AddRangeAsync(items, cancellationToken);
        }

        public async Task<T> CreateAsync(T item, CancellationToken cancellationToken = default)
        {
            return (await _dbSet.AddAsync(item, cancellationToken).ConfigureAwait(false)).Entity;
        }

        public void UpdateRange(ICollection<T> items)
        {
            if (items.Count == 0) return;
            foreach (var item in items)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return _dbSet.Where(expression).CountAsync(cancellationToken);
        }

        public async Task<ICollection<T>> GetAsync<TKey>(Expression<Func<T, bool>> expression, Expression<Func<T, TKey>> orderByExpression = null, int skip = 0, int take = int.MaxValue, CancellationToken cancellationToken = default)
        {
            if (orderByExpression != null)
            {
                return await _dbSet.Where(expression)
                    .OrderByDescending(orderByExpression)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            return await _dbSet.Where(expression)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }
    }
}