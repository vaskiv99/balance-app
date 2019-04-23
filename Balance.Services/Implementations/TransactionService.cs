using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Balance.DAL;
using Balance.DAL.Entities;
using Balance.Models.Enums;
using Balance.Models.Options;
using Balance.Models.Requests;
using Balance.Models.Responses;
using Balance.Models.Transactions;
using Balance.Services.Abstractions;
using Balance.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Balance.Services.Implementations
{
    public class TransactionService : BaseService, ITransactionService
    {
        #region Private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly TransactionOptions _options;

        #endregion

        #region Constructors

        public TransactionService(IUnitOfWork unitOfWork, IOptions<TransactionOptions> options)
        {
            _unitOfWork = unitOfWork;
            _options = options.Value;
        }

        #endregion

        #region ITransactionService members

        public async Task<OperationResponse<TransactionResponse>> ExecuteTransactionAsync(long userId,
            TransactionRequest request, TransactionType transactionType, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Amount <= 0)
            {
                return new OperationResponse<TransactionResponse>("Amount must be greater than 0.");
            }

            var retry = _options.CountOfRetry;
            while (--retry > 0)
            {
                try
                {
                    switch (transactionType)
                    {
                        case TransactionType.Deposit:
                            return await DepositAsync(userId, request, cancellationToken).ConfigureAwait(false);
                        case TransactionType.Withdraw:
                            return await WithdrawAsync(userId, request, cancellationToken).ConfigureAwait(false);
                        case TransactionType.Transfer:
                            return await TransferMoneyAsync(userId, request, cancellationToken).ConfigureAwait(false);
                        default: throw new ArgumentOutOfRangeException(nameof(transactionType));
                    }
                }
                catch (DbUpdateException)
                {
                    await Task.Delay(5, cancellationToken).ConfigureAwait(false);
                }
            }

            return new OperationResponse<TransactionResponse>("Too much attempt to execute transactions.");
        }

        public async Task<PaginatedResponse<TransactionResponse>> GetTransactionsAsync(long userId, PageQuery pageQuery,
            CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.TransactionRepository.CountAsync(x => x.SenderId == userId || x.RecipientId == userId, cancellationToken)
                .ConfigureAwait(false);

            var transactions = await _unitOfWork.TransactionRepository.GetAsync(x => x.SenderId == userId || x.RecipientId == userId,
                    x => x.CreatedDate,
                    pageQuery.PageIndex * pageQuery.PageSize, pageQuery.PageSize, cancellationToken)
                .ConfigureAwait(false);

            var response = transactions.Select(x => x.ToResponse());
            var paging = CreatePaging(pageQuery, count);

            return new PaginatedResponse<TransactionResponse>(response.ToList(), paging);
        }

        #endregion

        #region Private methods

        private async Task<OperationResponse<TransactionResponse>> TransferMoneyAsync(long senderId, TransactionRequest request, CancellationToken cancellationToken)
        {
            if (senderId == request.RecipientId)
            {
                return new OperationResponse<TransactionResponse>("Can not transfer money to yourself.");
            }

            var sender = await _unitOfWork.UserRepository.FindAsync(x => x.Id == senderId, cancellationToken)
                .ConfigureAwait(false);

            if (sender == null)
            {
                return new OperationResponse<TransactionResponse>("Sender not found.");
            }

            if (sender.Balance < request.Amount)
            {
                return new OperationResponse<TransactionResponse>("Not enough money on the balance.");
            }

            var recipient = await _unitOfWork.UserRepository.FindAsync(x => x.Id == request.RecipientId, cancellationToken)
                .ConfigureAwait(false);

            if (recipient == null)
            {
                return new OperationResponse<TransactionResponse>("Recipient not found.");
            }

            EnsureTransferMoney(sender, recipient, request);

            _unitOfWork.UserRepository.UpdateRange(new[] { sender, recipient });
            var entityEntry = await _unitOfWork.TransactionRepository.CreateAsync(request.ToEntity(TransactionType.Transfer, senderId),
                cancellationToken).ConfigureAwait(false);

            await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

            return new OperationResponse<TransactionResponse>(entityEntry.ToResponse());
        }

        private static void EnsureTransferMoney(UserEntity sender, UserEntity recipient, TransactionRequest request)
        {
            sender.Balance -= request.Amount;
            recipient.Balance += request.Amount;
        }

        private async Task<OperationResponse<TransactionResponse>> DepositAsync(long userId, TransactionRequest request, CancellationToken cancellationToken)
        {
            if (userId != request.RecipientId)
            {
                return new OperationResponse<TransactionResponse>("Can not execute deposit to other user.");
            }

            var user = await _unitOfWork.UserRepository.FindAsync(x => x.Id == request.RecipientId, cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                return new OperationResponse<TransactionResponse>("User not found.");
            }

            EnsureDepositMoney(user, request);

            _unitOfWork.UserRepository.Update(user);

            var entityEntry = await _unitOfWork.TransactionRepository.CreateAsync(request.ToEntity(TransactionType.Deposit, userId),
                cancellationToken).ConfigureAwait(false);

            await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

            return new OperationResponse<TransactionResponse>(entityEntry.ToResponse());
        }

        private static void EnsureDepositMoney(UserEntity user, TransactionRequest request)
        {
            user.Balance += request.Amount;
        }

        private async Task<OperationResponse<TransactionResponse>> WithdrawAsync(long userId, TransactionRequest request, CancellationToken cancellationToken)
        {
            if (userId != request.RecipientId)
            {
                return new OperationResponse<TransactionResponse>("Can not execute withdraw to other user.");
            }

            var user = await _unitOfWork.UserRepository.FindAsync(x => x.Id == request.RecipientId, cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                return new OperationResponse<TransactionResponse>("User not found.");
            }

            if (user.Balance < request.Amount)
            {
                return new OperationResponse<TransactionResponse>("Not enough money on the balance.");
            }

            EnsureWithdrawMoney(user, request);

            _unitOfWork.UserRepository.Update(user);

            var entityEntry = await _unitOfWork.TransactionRepository.CreateAsync(request.ToEntity(TransactionType.Withdraw, userId),
                cancellationToken).ConfigureAwait(false);

            await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

            return new OperationResponse<TransactionResponse>(entityEntry.ToResponse());
        }

        private static void EnsureWithdrawMoney(UserEntity user, TransactionRequest request)
        {
            user.Balance -= request.Amount;
        }

        #endregion
    }
}