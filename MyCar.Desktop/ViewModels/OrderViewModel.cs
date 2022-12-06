using ModelsApi;
using MyCar.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public List<OrderApi> Orders { get; set; }  

        public OrderViewModel()
        {
            Task.Run(GetOrders).Wait();
        }

        private async Task GetOrders()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
        }
    }
}
