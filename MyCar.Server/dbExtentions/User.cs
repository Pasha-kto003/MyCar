using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class User
    {
        public static explicit operator UserApi(User user)
        {
            return new UserApi
            {
                ID = user.Id,
                Email = user.Email,
                SaltHash = user.SaltHash,
                UserName = user.UserName,
                PassportId = user.PassportId,
                PasswordHash = user.PasswordHash,
                UserTypeId = user.UserTypeId
            };
        }
        public static explicit operator User(UserApi user)
        {
            return new User
            {
                Id = user.ID,
                SaltHash = user.SaltHash,
                Email = user.Email,
                UserName = user.UserName,
                PassportId = user.PassportId,
                PasswordHash = user.PasswordHash,
                UserTypeId = user.UserTypeId
            };
        }
    }
}
