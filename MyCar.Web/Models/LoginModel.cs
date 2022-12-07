using System.ComponentModel.DataAnnotations;

namespace MyCar.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан никнейм")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
