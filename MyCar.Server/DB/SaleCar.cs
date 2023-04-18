using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class SaleCar
    {
        public SaleCar()
        {
            CarPhotos = new HashSet<CarPhoto>();
            Warehouses = new HashSet<Warehouse>();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Articul { get; set; }
        public int? CarId { get; set; }
        public int? EquipmentId { get; set; }
        public decimal? EquipmentPrice { get; set; }
        public string? ColorCar { get; set; }
        public int? MinCount { get; set; }

        [ForeignKey("CarId")]
        public virtual Car? Car { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual Equipment? Equipment { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<CarPhoto> CarPhotos { get; set; }
        [Display(AutoGenerateField = false)]
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
