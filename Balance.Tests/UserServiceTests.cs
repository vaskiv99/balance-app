using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Balance.DAL;
using Balance.DAL.Entities;
using Balance.Models.Users;
using Balance.Services.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Balance.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IGeneralRepository<UserEntity, long>> _userRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = new Mock<IGeneralRepository<UserEntity, long>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _userService = new UserService(_unitOfWork.Object);
            _unitOfWork.Setup(uow => uow.UserRepository).Returns(_userRepository.Object);
        }

        [TestMethod]
        public async Task RegisterAsync_DuplicateUserName_ReturnErrors()
        {
            var request = new SignRequest
            {
                UserName = "admin",
                Password = "admin"
            };

            _unitOfWork.Setup(x => x.UserRepository.ExistAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(true);

            var result = await _userService.RegisterAsync(request, CancellationToken.None);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(result.IsSuccess);

            _unitOfWork.Verify(pr => pr.UserRepository.ExistAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));
            _unitOfWork.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Never);
        }

        [TestMethod]
        public async Task RegisterAsync_ValidObject_ReturnNewUser()
        {
            var request = new SignRequest
            {
                UserName = "admin",
                Password = "admin"
            };

            var user = new UserEntity
            {
                Id = 1,
                UserName = "admin",
                Password = "admin",
                Balance = 0
            };

            _unitOfWork.Setup(x => x.UserRepository.ExistAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(false);

            _unitOfWork.Setup(uow => uow.UserRepository.CreateAsync(It.IsAny<UserEntity>(), CancellationToken.None))
                .ReturnsAsync(user);

            var result = await _userService.RegisterAsync(request, CancellationToken.None);

            Assert.IsNotNull(result.Data);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.IsSuccess);

            _unitOfWork.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
            _unitOfWork.Verify(pr => pr.UserRepository.ExistAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));
            _unitOfWork.Verify(pr => pr.UserRepository.CreateAsync(It.IsAny<UserEntity>(), CancellationToken.None), Times.Once);
        }

        [TestMethod]
        public async Task GetUserInfoAsync_Valid_ReturnUserInfo()
        {
            var user = new UserEntity
            {
                Id = 1,
                UserName = "admin",
                Password = "admin",
                Balance = 0
            };

            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(user);

            var result = await _userService.GetUserInfoAsync(1, CancellationToken.None);

            Assert.IsNotNull(result.Data);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.IsSuccess);

            _unitOfWork.Verify(pr => pr.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task GetUserInfoAsync_UserNotFound_ReturnErrors()
        {
            _unitOfWork.Setup(x => x.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync((() => null));

            var result = await _userService.GetUserInfoAsync(1, CancellationToken.None);

            Assert.IsNull(result.Data);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(result.IsSuccess);

            _unitOfWork.Verify(pr => pr.UserRepository.FindAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task DepositAsync_WithNullRequestModel_ReturnErrors()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _userService.RegisterAsync(null, CancellationToken.None));
        }
    }
}
