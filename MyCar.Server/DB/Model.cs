using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class Model
    {
        public Model()
        {
            Cars = new HashSet<Car>();
        }

        public override string ToString()
        {
            return ModelName;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ModelName { get; set; } = null!;
        public int? MarkId { get; set; }

        [ForeignKey("MarkId")]
        public virtual MarkCar? Mark { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<Car> Cars { get; set; }
    }
}
