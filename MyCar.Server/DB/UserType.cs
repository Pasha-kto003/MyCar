using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class UserType
    {
        public UserType()
        {
            Users = new HashSet<User>();
        }

        public override string ToString()
        {
            return TypeName;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? TypeName { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<User> Users { get; set; }
    }
}
