using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Equipment
    {
        public static explicit operator EquipmentApi(Equipment equipment)
        {
            return new EquipmentApi
            {
                ID = equipment.Id,
                NameEquipment = equipment.NameEquipment
            };
        }

        public static explicit operator Equipment(EquipmentApi equipment)
        {
            return new Equipment
            {
                Id = equipment.ID,
                NameEquipment = equipment.NameEquipment
            };
        }
    }
}
