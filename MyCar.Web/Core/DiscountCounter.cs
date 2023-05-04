﻿using ModelsApi;

namespace MyCar.Web.Core
{
    public static class DiscountCounter
    {
        public static List<DiscountApi> Discounts { get; set; }
        public static List<WareHouseApi> WareHouses { get; set; }

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
                    if(date < discount.StartDate || date > discount.EndDate)
                      return finalPrice = 0;
                    if(discount.DiscountValue > 0 && discount.Price == 0)
                    {
                        var diff = saleCar.FullPrice * discount.DiscountValue / 100;
                        finalPrice = saleCar.FullPrice - diff;
                    }
                    if(discount.DiscountValue < 0 && discount.Price > 0)
                    {
                        finalPrice = saleCar.FullPrice - discount.Price;
                    }
                }
                return finalPrice;
            }
            return finalPrice;
        }

        public static decimal? GetCount(SaleCarApi saleCar) //данный метод сильно замедляет сайт
        {
            decimal? finalCount = 0;
            if (saleCar != null)
            {
                Task.Run(GetWare).Wait();
                var ware = WareHouses.Where(s => s.SaleCarId == saleCar.ID);
                //var date = DateTime.Now;
                if (ware != null)
                {
                    finalCount = ware.Sum(s => s.CountChange);
                }

                return finalCount;
            }
            return finalCount;
        }

        public static decimal? GetDiscountPrice(SaleCarApi saleCar)
        {
            var date = DateTime.Now;
            decimal? finalPrice = 0;
            if (saleCar != null)
            {
                Task.Run(GetList).Wait();
                var discount = Discounts.FirstOrDefault(s => s.SaleCarId == saleCar.ID);
                if (discount != null)
                {
                    if (discount.DiscountValue > 0 && discount.Price == 0)
                    {
                        finalPrice = saleCar.FullPrice * discount.DiscountValue / 100;
                    }
                    if (discount.DiscountValue < 0 && discount.Price > 0)
                    {
                        finalPrice = discount.Price;
                    }
                }
                return finalPrice;
            }
            return finalPrice;
        }

        public static async Task GetList()
        {
            Discounts = await Api.GetListAsync<List<DiscountApi>>("Discount");
            
        }

        private static async Task GetWare()
        {
            WareHouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
        }
    } 
}