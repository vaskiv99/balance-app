using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Balance.DAL;
using Balance.DAL.Entities;
using Balance.Services.Extensions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace Balance.Services.Implementations
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceOwnerPasswordValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.FindAsync(x => x.UserName == context.UserName);

                if (user == null)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
                    return;
                }

                if (user.Password == context.Password.CalculateMd5Hash())
                {
                    context.Result = new GrantValidationResult(
                        subject: user.Id.ToString(),
                        authenticationMethod: "custom",
                        claims: GetUserClaims(user));

                    return;
                }

                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
            }
            catch (Exception)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }

        public static IEnumerable<Claim> GetUserClaims(UserEntity user)
        {
            return new[]
            {
               new Claim(JwtClaimTypes.Id, user.Id.ToString()),
               new Claim(JwtClaimTypes.GivenName, user.UserName),
            };
        }       
    }
}