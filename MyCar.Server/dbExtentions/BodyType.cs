using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class BodyType
    {
        public static explicit operator BodyTypeApi(BodyType bodyType)
        {
            return new BodyTypeApi
            {
                ID = bodyType.Id,
                TypeName = bodyType.TypeName
            };
        }
        public static explicit operator BodyType(BodyTypeApi bodyType)
        {
            return new BodyType
            {
                Id = bodyType.ID,
                TypeName = bodyType.TypeName
            };
        }
    }
}
