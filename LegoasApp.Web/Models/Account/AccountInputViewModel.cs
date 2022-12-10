using System.ComponentModel.DataAnnotations;

namespace LegoasApp.Web.Models
{
    public class AccountInputViewModel
    {
        [Required(ErrorMessage = "Account name is required")]
        public string AccountName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password length minimum is 6")]
        public string Password { get; set; }
    }
}
