using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class MarkCar
    {
        public static explicit operator MarkCarApi(MarkCar markCar)
        {
            return new MarkCarApi
            {
                ID = markCar.Id,
                MarkName = markCar.MarkName,
                MarkPhoto = markCar.MarkPhoto
            };
        }

        public static explicit operator MarkCar(MarkCarApi markCar)
        {
            return new MarkCar
            {
                Id = markCar.ID,
                MarkName = markCar.MarkName,
                MarkPhoto = markCar.MarkPhoto
            };
        }
    }
}
