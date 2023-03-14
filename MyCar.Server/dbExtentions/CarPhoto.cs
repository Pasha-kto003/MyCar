using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class CarPhoto
    {
        public static explicit operator CarPhotoApi(CarPhoto carPhoto)
        {
            return new CarPhotoApi
            {
                ID = carPhoto.Id,
                PhotoName = carPhoto.PhotoName,
                SaleCarId = carPhoto.SaleCarId,
                IsMainPhoto = carPhoto.IsMainPhoto
            };
        }
        public static explicit operator CarPhoto(CarPhotoApi carPhoto)
        {
            return new CarPhoto
            {
                Id = carPhoto.ID,
                PhotoName = carPhoto.PhotoName,
                SaleCarId = carPhoto.SaleCarId,
                IsMainPhoto= carPhoto.IsMainPhoto
            };
        }
    }
}
