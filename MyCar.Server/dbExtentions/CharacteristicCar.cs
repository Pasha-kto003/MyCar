using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class CharacteristicCar
    {
        public static explicit operator CharacteristicCarApi(CharacteristicCar characteristic)
        {
            return new CharacteristicCarApi
            {
                CarId = characteristic.CarId,
                CharacteristicId = characteristic.CharacteristicId,
                CharacteristicValue = characteristic.CharacteristicValue
            };
        }

        public static explicit operator CharacteristicCar(CharacteristicCarApi characteristic)
        {
            return new CharacteristicCar
            {
                CarId = characteristic.CarId,
                CharacteristicId = characteristic.CharacteristicId,
                CharacteristicValue = characteristic.CharacteristicValue
            };
        }
    }
}
