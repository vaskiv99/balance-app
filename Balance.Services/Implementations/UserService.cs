using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Balance.DAL;
using Balance.Models.Requests;
using Balance.Models.Responses;
using Balance.Models.Users;
using Balance.Services.Abstractions;
using Balance.Services.Extensions;

namespace Balance.Services.Implementations
{
    public class UserService : BaseService, IUserService
    {
        #region Private fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region IUserService members

        public async Task<OperationResponse<UserResponse>> RegisterAsync(SignRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var isExistUser = await _unitOfWork.UserRepository
                .ExistAsync(x => x.UserName == request.UserName, cancellationToken).ConfigureAwait(false);

            if (isExistUser)
            {
                return new OperationResponse<UserResponse>("User with the same username already exist!");
            }

            var entryEntity = await _unitOfWork.UserRepository.CreateAsync(request.ToEntity(), cancellationToken)
                .ConfigureAwait(false);

            await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

            return new OperationResponse<UserResponse>(entryEntity.ToResponse());
        }

        public async Task<OperationResponse<UserResponse>> GetUserInfoAsync(long userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(x => x.Id == userId, cancellationToken)
                .ConfigureAwait(false);

            return user == null ?
                new OperationResponse<UserResponse>("User not found!") :
                new OperationResponse<UserResponse>(user.ToResponse());
        }

        public async Task<PaginatedResponse<UserResponse>> GetUsersAsync(PageQuery pageQuery,
            CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.UserRepository.CountAsync(_ => true, cancellationToken).ConfigureAwait(false);

            var users = await _unitOfWork.UserRepository
                .GetAsync<long>(_ => true, null, pageQuery.PageIndex * pageQuery.PageSize, pageQuery.PageSize, cancellationToken)
                .ConfigureAwait(false);

            var response = users.Select(x => x.ToResponse());
            var paging = CreatePaging(pageQuery, count);

            return new PaginatedResponse<UserResponse>(response.ToList(), paging);
        }

        public async Task<IEnumerable<UserResponse>> GetUsersByIdsAsync(IEnumerable<long> ids,
            CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.UserRepository.GetAsync<long>(x => ids.Contains(x.Id), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return users.Select(x => x.ToResponse());
        }

        #endregion
    }
}