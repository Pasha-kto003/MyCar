using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class MarkCar
    {
        public MarkCar()
        {
            Models = new HashSet<Model>();
        }

        public int Id { get; set; }
        public string? MarkName { get; set; }

        public virtual ICollection<Model> Models { get; set; }
    }
}
