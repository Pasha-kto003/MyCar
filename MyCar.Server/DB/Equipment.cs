using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCar.Server.DB
{
    public partial class Equipment
    {
        public Equipment()
        {
            SaleCars = new HashSet<SaleCar>();
        }

        public override string ToString()
        {
            return NameEquipment;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? NameEquipment { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<SaleCar> SaleCars { get; set; }
    }
}
