using System.Linq;
using System.Threading.Tasks;
using Balance.DAL;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Balance.Services.Implementations
{
    public class IdentityClaimsProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IdentityClaimsProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var user = await _unitOfWork.UserRepository.FindAsync(x => x.Id == long.Parse(subjectId)).ConfigureAwait(false);

            var claims = ResourceOwnerPasswordValidator.GetUserClaims(user);

            context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();

            if (!long.TryParse(subjectId, out var id))
            {
                context.IsActive = false;
                return;
            }

            var user = await _unitOfWork.UserRepository.FindAsync(x => x.Id == id).ConfigureAwait(false);
            context.IsActive = user != null;
        }
    }
}