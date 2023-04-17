using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCar.Server.DB
{
    public partial class Warehouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? SaleCarId { get; set; }
        public int? OrderId { get; set; }
        public int? CountChange { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("SaleCarId")]
        public virtual SaleCar? SaleCar { get; set; }
    }
}
