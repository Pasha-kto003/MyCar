using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Passport
    {
        public Passport()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? Seria { get; set; }
        public string? Number { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
