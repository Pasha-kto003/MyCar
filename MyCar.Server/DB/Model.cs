using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Model
    {
        public Model()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string ModelName { get; set; } = null!;
        public int? MarkId { get; set; }

        public virtual MarkCar? Mark { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}
