﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class UserApi : ApiBaseType
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronimyc { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public int? PassportId { get; set; }
        public int? UserTypeId { get; set; }

        public virtual PassportApi? Passport { get; set; }
        public virtual UserTypeApi? UserType { get; set; }
    }
}
