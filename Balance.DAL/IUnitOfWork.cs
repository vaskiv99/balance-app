using System.Threading;
using System.Threading.Tasks;
using Balance.DAL.Entities;

namespace Balance.DAL
{
    public interface IUnitOfWork
    {
        IGeneralRepository<UserEntity, long> UserRepository { get; }
        IGeneralRepository<TransactionEntity, long> TransactionRepository { get; }
        Task CommitAsync(CancellationToken cancellationToken);
    }
}