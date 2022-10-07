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
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronimyc = user.Patronimyc,
                Email = user.Email,
                UserName = user.UserName,
                PassportId = user.PassportId,
                Password = user.Password,
                UserTypeId = user.UserTypeId
            };
        }
        public static explicit operator User(UserApi user)
        {
            return new User
            {
                Id = user.ID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronimyc = user.Patronimyc,
                Email = user.Email,
                UserName = user.UserName,
                PassportId = user.PassportId,
                Password = user.Password,
                UserTypeId = user.UserTypeId
            };
        }
    }
}
