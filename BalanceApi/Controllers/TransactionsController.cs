using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Balance.Infrastructure.Extensions;
using Balance.Models.Enums;
using Balance.Models.Requests;
using Balance.Models.Responses;
using Balance.Models.Transactions;
using Balance.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Balance.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<OperationResponse<TransactionResponse>> DepositAsync([Required][FromBody] TransactionRequest request,
            CancellationToken cancellationToken)
        {
            return await _transactionService.ExecuteTransactionAsync(this.GetAuthUserId(), request, TransactionType.Deposit, cancellationToken)
                .ConfigureAwait(false);
        }

        [HttpPost("withdraw")]
        public async Task<OperationResponse<TransactionResponse>> WithdrawAsync([Required][FromBody] TransactionRequest request,
            CancellationToken cancellationToken)
        {
            return await _transactionService.ExecuteTransactionAsync(this.GetAuthUserId(), request, TransactionType.Withdraw, cancellationToken)
                .ConfigureAwait(false);
        }

        [HttpPost("transfer")]
        public async Task<OperationResponse<TransactionResponse>> TransferMoneyAsync([Required][FromBody] TransactionRequest request,
            CancellationToken cancellationToken)
        {
            return await _transactionService.ExecuteTransactionAsync(this.GetAuthUserId(), request, TransactionType.Transfer, cancellationToken)
                .ConfigureAwait(false);
        }

        [HttpGet]
        public async Task<PaginatedResponse<TransactionResponse>> GetTransactionsAsync([FromQuery] PageQuery pageQuery,
            CancellationToken cancellationToken)
        {
            return await _transactionService.GetTransactionsAsync(this.GetAuthUserId(), pageQuery, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
