using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models
{
    public class User : IdentityUser
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string City { get; set; }
        public DateTime DateBirth {get;set;}
        public string Login { get; set; }
        public DateTime DateRegistration { get; set; }
        public bool Status { get; set; }
    }
}
