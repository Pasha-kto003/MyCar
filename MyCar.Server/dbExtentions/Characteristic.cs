using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Characteristic
    {
        public static explicit operator CharacteristicApi(Characteristic characteristic)
        {
            return new CharacteristicApi
            {
                ID = characteristic.Id,
                CharacteristicName = characteristic.CharacteristicName,
                UnitId = characteristic.UnitId
            };
        }

        public static explicit operator Characteristic(CharacteristicApi characteristic)
        {
            return new Characteristic
            {
                Id = characteristic.ID,
                CharacteristicName = characteristic.CharacteristicName,
                UnitId = characteristic.UnitId
            };
        }
    }
}
