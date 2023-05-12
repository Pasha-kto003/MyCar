using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Discount
    {
        public static explicit operator DiscountApi(Discount discount)
        {
            return new DiscountApi
            {
                ID = discount.Id,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                DiscountValue = discount.DiscountValue,
                SaleCarId = discount.SaleCarId
            };
        }

        public static explicit operator Discount(DiscountApi discount)
        {
            return new Discount
            {
                Id = discount.ID,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                DiscountValue = discount.DiscountValue,
                SaleCarId = discount.SaleCarId
            };
        }
    }
}
