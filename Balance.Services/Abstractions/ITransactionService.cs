using System.Threading;
using System.Threading.Tasks;
using Balance.Models.Enums;
using Balance.Models.Requests;
using Balance.Models.Responses;
using Balance.Models.Transactions;

namespace Balance.Services.Abstractions
{
    public interface ITransactionService
    {
        Task<OperationResponse<TransactionResponse>> ExecuteTransactionAsync(long userId,
            TransactionRequest request, TransactionType transactionType, CancellationToken cancellationToken);

        Task<PaginatedResponse<TransactionResponse>> GetTransactionsAsync(long userId, PageQuery pageQuery,
            CancellationToken cancellationToken);
    }
}