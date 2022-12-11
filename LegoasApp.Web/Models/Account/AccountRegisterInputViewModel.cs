using LegoasApp.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegoasApp.Web.Models
{
    public class AccountRegisterInputViewModel
    {
        [Required(ErrorMessage = "Account name is required")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Minimum length is 4, and maximum length is 50")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Minimum length is 6")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public List<int> Roles { get; set; }

        [Required(ErrorMessage = "Menu access is required")]
        public List<int> MenuAccess { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        public List<int> Branches { get; set; }

        //Optional
        public string? UserName { get; set; }

        public string? Address { get; set; }

        public string? PostalCode { get; set; }

        public string? Province { get; set; }

        // Display Dropdown
        public MultiSelectList? RoleList { get; set; }

        public MultiSelectList? MenuAccessList { get; set; }

        public MultiSelectList? BranchList { get; set; }
    }
}
