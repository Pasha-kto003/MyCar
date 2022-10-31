using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class OrderApi : ApiBaseType
    {
        public DateTime? DateOfOrder { get; set; }
        public int? ClientId { get; set; }
        public int? ActionTypeId { get; set; }
        public int? StatusId { get; set; }

        public List<WareHouseApi> WareHouses { get; set; }
        public List<CarApi> Cars { get; set; }
    }
}
