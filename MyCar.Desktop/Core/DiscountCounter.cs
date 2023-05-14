using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.Core
{
    public static class DiscountCounter
    {
        public static List<DiscountApi> Discounts { get; set; }
        /// <summary>
        /// Рассчет цены машины после скидки
        /// </summary>
        public static decimal? GetDiscount(SaleCarApi saleCar)
        {
            var date = DateTime.Now;
            decimal? finalPrice = 0;
            if (saleCar != null)
            {
                Task.Run(GetList).Wait();
                var discount = Discounts.FirstOrDefault(s => s.SaleCarId == saleCar.ID);
                //var date = DateTime.Now;
                if (discount != null)
                {
                    if (date < discount.StartDate || date > discount.EndDate)
                        return finalPrice = 0;
                    if (discount.Price > 0)
                    {
                        finalPrice = saleCar.FullPrice - discount.Price;
                    }
                    if (discount.Price < 0)
                    {
                        finalPrice = saleCar.FullPrice;
                    }
                }
                return finalPrice;
            }
            return finalPrice;
        }

        /// <summary>
        /// Метод для получения процента скидки
        /// </summary>
        public static decimal? GetPercent(SaleCarApi saleCar)
        {
            decimal? discountPercent = 0;
            if (saleCar != null)
            {
                Task.Run(GetList).Wait();
                var discount = Discounts.FirstOrDefault(s => s.SaleCarId == saleCar.ID);
                //var date = DateTime.Now;
                if (discount != null)
                {
                    if (discount.Price > 0)
                    {
                        discountPercent = saleCar.FullPrice * 100 / discount.Price;
                    }
                    else
                    {
                        discountPercent = 0;
                    }
                }
                return discountPercent;
            }
            return discountPercent;
        }

        public static async Task GetList()
        {
            Discounts = await Api.GetListAsync<List<DiscountApi>>("Discount");
        }
    }
}
