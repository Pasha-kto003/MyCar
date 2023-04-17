using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class ActionType
    {
        public ActionType()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? ActionTypeName { get; set; }

        public override string ToString()
        {
            return ActionTypeName;
        }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
