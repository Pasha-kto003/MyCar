using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCar.Server.DB
{
    public partial class Characteristic
    {
        public Characteristic()
        {
            CharacteristicCars = new HashSet<CharacteristicCar>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? CharacteristicName { get; set; }
        public int? UnitId { get; set; }

        [ForeignKey("UnitId")]
        public virtual Unit? Unit { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<CharacteristicCar> CharacteristicCars { get; set; }
    }
}
