namespace MyCar.Web.Models
{
    public class User
    {
        public int ID { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int? PassportId { get; set; }
        public int? UserTypeId { get; set; }
        public string Password { get; set; }

        public UserTypeModel UserType { get; set; }

        public PassportModel Passport { get; set; }
    }
}
