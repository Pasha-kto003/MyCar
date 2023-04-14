using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCar.Server.DB
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public byte[]? PasswordHash { get; set; }
        public string? Email { get; set; }
        public int? PassportId { get; set; }
        public int? UserTypeId { get; set; }
        public byte[]? SaltHash { get; set; }

        public override string ToString()
        {
            return UserName;
        }

        [ForeignKey("PassportId")]
        public virtual Passport? Passport { get; set; }

        [ForeignKey("UserTypeId")]
        public virtual UserType? UserType { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
