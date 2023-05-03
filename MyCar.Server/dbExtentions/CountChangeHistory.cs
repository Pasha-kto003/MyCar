using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class CountChangeHistory
    {
        public static explicit operator CountChangeHistoryApi(CountChangeHistory countChange)
        {
            return new CountChangeHistoryApi
            {
                ID = countChange.Id,
                Count = countChange.Count,
                WarehouseInId = countChange.WarehouseInId,
                WarehouseOutId = countChange.WarehouseOutId
            };
        }
        public static explicit operator CountChangeHistory(CountChangeHistoryApi countChange)
        {
            return new CountChangeHistory
            {
                Id = countChange.ID,
                Count = countChange.Count,
                WarehouseInId = countChange.WarehouseInId,
                WarehouseOutId = countChange.WarehouseOutId
            };
        }
    }
}
