using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCar.Server.DB
{
    public partial class Status
    {
        public Status()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? StatusName { get; set; }

        public override string ToString()
        {
            return StatusName;
        }


        [Display(AutoGenerateField = false)]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
