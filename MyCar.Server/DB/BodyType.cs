using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class BodyType
    {
        public BodyType()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string? TypeName { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
