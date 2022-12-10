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
        [MinLength(1, ErrorMessage = "At least select 1 role")]
        public List<int> Roles { get; set; }

        [Required(ErrorMessage = "Menu access is required")]
        [MinLength(1, ErrorMessage = "At least select 1 menu access")]
        public List<int> MenuAccess { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        [MinLength(1, ErrorMessage = "At least select 1 branch")]
        public List<int> Branches { get; set; }

        //Optional
        [StringLength(255, ErrorMessage = "Max length is 255")]
        public string UserName { get; set; }

        public string Address { get; set; }

        [StringLength(10, ErrorMessage = "Max length is 10")]
        public string PostalCode { get; set; }

        [StringLength(50, ErrorMessage = "Max length is 50")]
        public string Province { get; set; }
    }
}
