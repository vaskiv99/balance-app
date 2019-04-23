using Balance.DAL.Entities;
using Balance.Models.Users;

namespace Balance.Services.Extensions
{
    public static class UsersExtension
    {
        public static UserEntity ToEntity(this SignRequest request) => new UserEntity
        {
            Balance = 0,
            Password = request.Password.CalculateMd5Hash(),
            UserName = request.UserName
        };

        public static UserResponse ToResponse(this UserEntity entity) => new UserResponse
        {
            Id = entity.Id,
            Balance = entity.Balance,
            UserName = entity.UserName
        };
    }
}