using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.Core
{
    public class DataCar
    {
        public bool IsSelected { get; set; } = false;

        public string CarName { get; set; }

        public List<DataSaleCarColor> DataSaleCars { get; set; }
    }
    public class DataSaleCar
    {
        public bool IsSelected { get; set; } = false;

        public string CarName { get; set; }

        public string EquipmentName { get; set; }

        public List<DataSaleCarColor> DataSaleCarColors { get; set; }
    }

    public class DataSaleCarColor
    {
        public bool IsSelected { get; set; } = false;

        public string CarName { get; set; }

        public string EquipmentName { get; set; }

        public string ColorName { get; set; }

    }
}
