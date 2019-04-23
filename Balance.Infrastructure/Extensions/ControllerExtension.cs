using System.Linq;
using System.Security.Claims;
using Balance.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Balance.Infrastructure.Extensions
{
    public static class ControllerExtension
    {
        public static long GetAuthUserId(this ControllerBase controller)
        {
            if(!long.TryParse(controller.User.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier || cl.Type == "sub")?.Value, out var userId))
            {
                throw new InvalidPermissionException();
            }

            return userId;
        }
    }
}