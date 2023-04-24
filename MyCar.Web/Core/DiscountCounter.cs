using ModelsApi;

namespace MyCar.Web.Core
{
    public static class DiscountCounter
    {
        public static decimal? GetDiscount(SaleCarApi saleCar)
        {
            var date = DateTime.Now;
            decimal? finalPrice = 0;
            if (saleCar != null)
            {
                if (date.DayOfWeek == DayOfWeek.Monday)
                {
                    //ViewBag.DiscountPrice = 
                    if (saleCar.Car.CarMark.Contains("Toyota") || saleCar.Car.CarMark.Contains("Lexus") || saleCar.Car.CarMark.Contains("Honda"))
                    {
                        var diff = saleCar.FullPrice * 10 / 100;
                        finalPrice = saleCar.FullPrice - diff;
                        return finalPrice;
                    }
                    else
                    {
                        finalPrice = 0;
                    }
                }
                else if (date.DayOfWeek == DayOfWeek.Tuesday || date.DayOfWeek == DayOfWeek.Friday)
                {
                    if (saleCar.Car.CarMark.Contains("Porsche") || saleCar.Car.CarMark.Contains("Audi") || saleCar.Car.CarMark.Contains("Honda"))
                    {
                        var diff = saleCar.FullPrice * 10 / 100;
                        finalPrice = saleCar.FullPrice - diff;
                        return finalPrice;
                    }
                    else
                    {
                        return finalPrice = 0;
                    }
                }
                else
                {
                    return finalPrice = 0;
                }

                return finalPrice;
            }
            return finalPrice;
        }

    } 
}
