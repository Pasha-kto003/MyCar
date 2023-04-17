using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyCar.Server.DB
{
    public partial class Passport
    {
        public Passport()
        {
            Users = new HashSet<User>();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Seria { get; set; }
        public string? Number { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }

        [Display(AutoGenerateField = false)]
        public virtual ICollection<User> Users { get; set; }
    }
}
