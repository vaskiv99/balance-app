using System.Threading;
using System.Threading.Tasks;
using Balance.DAL.Entities;

namespace Balance.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Private fields

        private readonly BalanceDbContext _context;
        private IGeneralRepository<UserEntity, long> _userRepository;
        private IGeneralRepository<TransactionEntity, long> _transactionRepository;

        #endregion

        #region Constructors

        public UnitOfWork(BalanceDbContext context)
        {
            _context = context;
        }

        #endregion

        #region IUnitOfWork members

        public IGeneralRepository<UserEntity, long> UserRepository
        {
            get
            {
                return _userRepository = _userRepository ?? new GeneralRepository<UserEntity, long>(_context);
            }
        }

        public IGeneralRepository<TransactionEntity, long> TransactionRepository
        {
            get
            {
                return _transactionRepository = _transactionRepository ?? new GeneralRepository<TransactionEntity, long>(_context);
            }
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}