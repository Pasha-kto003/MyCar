using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class Order
    {
        public Order()
        {
            Warehouses = new HashSet<Warehouse>();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? DateOfOrder { get; set; }
        public int? UserId { get; set; }
        public int? ActionTypeId { get; set; }
        public int? StatusId { get; set; }

        [ForeignKey("ActionTypeId")]
        public virtual ActionType? ActionType { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
