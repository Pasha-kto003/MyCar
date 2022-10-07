using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class UserType
    {
        public static explicit operator UserTypeApi(UserType userType) 
        {
            return new UserTypeApi
            {
                ID = userType.Id,
                TypeName = userType.TypeName
            };
        }
        public static explicit operator UserType(UserTypeApi userType)
        {
            return new UserType
            {
                Id = userType.ID,
                TypeName = userType.TypeName,
            };
        }
    }
}
