using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Balance.Models.Requests;
using Balance.Models.Responses;
using Balance.Models.Users;
using Balance.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Balance.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<OperationResponse<UserResponse>> RegisterAsync([Required][FromBody] SignRequest request,
            CancellationToken cancellationToken)
        {
            return await _userService.RegisterAsync(request, cancellationToken).ConfigureAwait(false);
        }

        [HttpGet("{userId:long}")]
        public async Task<OperationResponse<UserResponse>> GetUserInfoAsync([FromRoute] long userId,
            CancellationToken cancellationToken)
        {
            return await _userService.GetUserInfoAsync(userId, cancellationToken).ConfigureAwait(false);
        }

        [HttpGet]
        public async Task<PaginatedResponse<UserResponse>> GetUsersAsync([FromQuery] PageQuery pageQuery, CancellationToken cancellationToken)
        {
            return await _userService.GetUsersAsync(pageQuery, cancellationToken).ConfigureAwait(false);
        }

        [HttpGet("by-ids")]
        public async Task<IEnumerable<UserResponse>> GetUsersByIdsAsync([FromQuery] IEnumerable<long> ids, CancellationToken cancellationToken)
        {
            return await _userService.GetUsersByIdsAsync(ids, cancellationToken).ConfigureAwait(false);
        }
    }
}