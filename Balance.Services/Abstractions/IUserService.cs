using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Balance.Models.Requests;
using Balance.Models.Responses;
using Balance.Models.Users;

namespace Balance.Services.Abstractions
{
    public interface IUserService
    {
        Task<OperationResponse<UserResponse>> RegisterAsync(SignRequest request, CancellationToken cancellationToken);

        Task<OperationResponse<UserResponse>> GetUserInfoAsync(long userId, CancellationToken cancellationToken);

        Task<PaginatedResponse<UserResponse>> GetUsersAsync(PageQuery pageQuery, CancellationToken cancellationToken);

        Task<IEnumerable<UserResponse>> GetUsersByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken);
    }
}