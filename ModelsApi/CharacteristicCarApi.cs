using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class CharacteristicCarApi
    {
        public int CarId { get; set; }
        public int CharacteristicId { get; set; }
        public string? CharacteristicValue { get; set; }

        public CharacteristicApi Characteristic { get; set; }
    }
}
