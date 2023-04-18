using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class Color
    {
        public string Name { get; set; }
        public string HexCode { get; set; }

        public Color(string name, string hexCode)
        {
            Name = name;
            HexCode = hexCode;
        }
    }
}
