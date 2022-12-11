using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class UserApi : ApiBaseType
    {       
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int? PassportId { get; set; }
        public int? UserTypeId { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? SaltHash { get; set; }

        public virtual UserTypeApi? UserType { get; set; }

        public virtual PassportApi? Passport { get; set; }
    }
}
