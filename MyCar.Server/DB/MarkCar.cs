using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class MarkCar
    {
        public MarkCar()
        {
            Models = new HashSet<Model>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? MarkName { get; set; }

        public override string ToString()
        {
            return MarkName;
        }

        public string? MarkPhoto { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<Model> Models { get; set; }
    }
}
