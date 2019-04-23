using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Balance.DAL;
using Balance.DAL.Entities;
using Balance.Models.Enums;
using Balance.Models.Options;
using Balance.Models.Transactions;
using Balance.Services.Implementations;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Balance.Tests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private TransactionService _transactionService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IGeneralRepository<TransactionEntity, long>> _transactionRepository;
        private Mock<IOptions<TransactionOptions>> _mockOptions;

        [TestInitialize]
        public void Setup()
        {
            _mockOptions = new Mock<IOptions<TransactionOptions>>();
            _mockOptions.Setup(x => x.Value).Returns(new TransactionOptions {CountOfRetry = 5});
            _transactionRepository = new Mock<IGeneralRepository<TransactionEntity, long>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _transactionService = new TransactionService(_unitOfWork.Object, _mockOptions.Object);
            _unitOfWork.Setup(uow => uow.TransactionRepository).Returns(_transactionRepository.Object);
        }

        #region Validator

        [TestMethod]
        public async Task DepositAsync_WithNullRequestModel_ReturnErrors()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _transactionService.ExecuteTransactionAsync(2, null, TransactionType.Deposit, CancellationToken.None).ConfigureAwait(false));
        }

        #endregion

        #region Deposit tests

        [TestMethod]
        public async Task DepositAsync_ToYourself_ReturnErrors()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            var result = await _transactionService.ExecuteTransactionAsync(2, request, TransactionType.Deposit, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Can not execute deposit to other user.", result.Error?.Message);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task DepositAsync_UserNotFound_ReturnErrors()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync((() => null));

            var result = await _transactionService.ExecuteTransactionAsync(1, request, TransactionType.Deposit, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("User not found.", result.Error?.Message);
            Assert.IsFalse(result.IsSuccess);

            _unitOfWork.Verify(pr => pr.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task DepositAsync_Valid_ReturnTransaction()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new UserEntity
                {
                    Id = 1,
                    UserName = "admin",
                    Balance = 21
                });

            _unitOfWork.Setup(uow => uow.UserRepository.Update(It.IsAny<UserEntity>()));
            _unitOfWork.Setup(uow => uow.TransactionRepository.CreateAsync(It.IsAny<TransactionEntity>(), CancellationToken.None))
                .ReturnsAsync(new TransactionEntity
                {
                    Id = 1,
                    Amount = request.Amount,
                    CreatedDate = DateTimeOffset.UtcNow,
                    RecipientId = request.RecipientId,
                    SenderId = request.RecipientId,
                    TransactionType = TransactionType.Deposit
                });

            var result = await _transactionService.ExecuteTransactionAsync(1, request, TransactionType.Deposit, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNotNull(result.Data);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(request.Amount, result.Data.Amount);
        }

        #endregion

        #region Withdraw

        [TestMethod]
        public async Task WithdrawAsync_UserNotFound_ReturnErrors()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync((() => null));

            var result = await _transactionService.ExecuteTransactionAsync(1, request, TransactionType.Withdraw, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("User not found.", result.Error?.Message);
            Assert.IsFalse(result.IsSuccess);

            _unitOfWork.Verify(pr => pr.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));

        }

        [TestMethod]
        public async Task WithdrawAsync_NotEnoughMoney_ReturnErrors()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new UserEntity
                {
                    Id = 1,
                    UserName = "admin",
                    Balance = 21
                });

            var result = await _transactionService.ExecuteTransactionAsync(1, request, TransactionType.Withdraw, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Not enough money on the balance.", result.Error?.Message);
            Assert.IsFalse(result.IsSuccess);

            _unitOfWork.Verify(pr => pr.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task WithdrawAsync_Valid_ReturnTransaction()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new UserEntity
                {
                    Id = 1,
                    UserName = "admin",
                    Balance = 300
                });

            _unitOfWork.Setup(uow => uow.UserRepository.Update(It.IsAny<UserEntity>()));
            _unitOfWork.Setup(uow => uow.TransactionRepository.CreateAsync(It.IsAny<TransactionEntity>(), CancellationToken.None))
                .ReturnsAsync(new TransactionEntity
                {
                    Id = 1,
                    Amount = request.Amount,
                    CreatedDate = DateTimeOffset.UtcNow,
                    RecipientId = request.RecipientId,
                    SenderId = request.RecipientId,
                    TransactionType = TransactionType.Withdraw
                });

            var result = await _transactionService.ExecuteTransactionAsync(1, request, TransactionType.Withdraw, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNotNull(result.Data);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(request.Amount, result.Data.Amount);
        }

        #endregion

        #region Transfer

        [TestMethod]
        public async Task TransferAsync_ToYourself_ReturnErrors()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            var result = await _transactionService.ExecuteTransactionAsync(1, request, TransactionType.Transfer, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Can not transfer money to yourself.", result.Error?.Message);
            Assert.IsFalse(result.IsSuccess);           
        }

        [TestMethod]
        public async Task TransferAsync_UserNotFound_ReturnErrors()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync((() => null));

            var result = await _transactionService.ExecuteTransactionAsync(2, request, TransactionType.Transfer, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Sender not found.", result.Error?.Message);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task TransferAsync_NotEnoughMoney_ReturnErrors()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new UserEntity
                {
                    Id = 1,
                    UserName = "admin",
                    Balance = 21
                });

            var result = await _transactionService.ExecuteTransactionAsync(2, request, TransactionType.Transfer, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Not enough money on the balance.", result.Error?.Message);
            Assert.IsFalse(result.IsSuccess);

            _unitOfWork.Verify(pr => pr.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task TransferAsync_Valid_ReturnTransaction()
        {
            var request = new TransactionRequest
            {
                Amount = 100,
                RecipientId = 1
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new UserEntity
                {
                    Id = 1,
                    UserName = "admin",
                    Balance = 300
                });

            _unitOfWork.Setup(uow => uow.UserRepository.UpdateRange(It.IsAny<ICollection<UserEntity>>()));
            _unitOfWork.Setup(uow => uow.TransactionRepository.CreateAsync(It.IsAny<TransactionEntity>(), CancellationToken.None))
                .ReturnsAsync(new TransactionEntity
                {
                    Id = 1,
                    Amount = request.Amount,
                    CreatedDate = DateTimeOffset.UtcNow,
                    RecipientId = request.RecipientId,
                    SenderId = request.RecipientId,
                    TransactionType = TransactionType.Transfer
                });

            var result = await _transactionService.ExecuteTransactionAsync(2, request, TransactionType.Transfer, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNotNull(result.Data);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(request.Amount, result.Data.Amount);
        }


        #endregion
    }
}