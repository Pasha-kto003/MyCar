using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class UserType
    {
        public UserType()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string? TypeName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
