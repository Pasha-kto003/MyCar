using ModelsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.Core
{
    public static class Configuration
    {
        public static UserApi CurrentUser { get; set; }

        public static Action CloseMainWindow { get; set; }

    }
}
