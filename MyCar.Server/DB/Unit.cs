using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class Unit
    {
        public Unit()
        {
            Characteristics = new HashSet<Characteristic>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UnitName { get; set; }

        public override string ToString()
        {
            return UnitName;
        }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<Characteristic> Characteristics { get; set; }
    }
}
