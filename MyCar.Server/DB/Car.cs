using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCar.Server.DB
{
    public partial class Car
    {
        public Car()
        {
            CharacteristicCars = new HashSet<CharacteristicCar>();
            SaleCars = new HashSet<SaleCar>();
        }

        public override string ToString()
        {
            return ModelId.ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ModelId { get; set; }
        public int? TypeId { get; set; }
        public string? Articul { get; set; }
        public decimal? CarPrice { get; set; }
        public string? PhotoCar { get; set; }

        [ForeignKey("ModelId")]
        public virtual Model? Model { get; set; }

        [ForeignKey("TypeId")]
        public virtual BodyType? Type { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<CharacteristicCar> CharacteristicCars { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<SaleCar> SaleCars { get; set; }
    }
}
