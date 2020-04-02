using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCollections.Models.ViewModel
{
    public class ManageUser : IdentityUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Blocked { get; set; }
    }
}
