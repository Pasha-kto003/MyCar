namespace MyCar.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public int? UserTypeId { get; set; }
        public UserTypeModel UserType { get; set; }
    }
}
