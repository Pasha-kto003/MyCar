using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Unit
    {
        public static explicit operator UnitApi(Unit unit)
        {
            return new UnitApi
            {
                ID = unit.Id,
                UnitName = unit.UnitName
            };
        }
        public static explicit operator Unit(UnitApi unit)
        {
            return new Unit
            {
                Id = unit.ID,
                UnitName = unit.UnitName
            };
        }
    }
}
