using System.ComponentModel.DataAnnotations;

namespace PersonalCollections.Models.Admin
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
