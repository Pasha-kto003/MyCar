namespace MyCar.Web.Models
{
    public class UserTypeModel
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public List<User> Users { get; set; }
        public UserTypeModel()
        {
            Users = new List<User>();
        }
    }
}
