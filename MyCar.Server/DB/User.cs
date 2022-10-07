using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronimyc { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public int? PassportId { get; set; }
        public int? UserTypeId { get; set; }

        public virtual Passport? Passport { get; set; }
        public virtual UserType? UserType { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
