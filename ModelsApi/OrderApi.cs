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
        public int? UserId { get; set; }
        public int? ActionTypeId { get; set; }
        public int? StatusId { get; set; }
        public string CarOptions { get; set; }

        public UserApi User { get; set; }
        public ActionTypeApi ActionType { get; set; }
        public StatusApi Status { get; set; }
        public List<WareHouseApi> WareHouses { get; set; }

        public decimal? SumOrder { get; set; }
    }
}
