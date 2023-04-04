using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GoodNewsAggregator.MVC.Models
{
    public class UserRegisterModel
    {
        [Required]
        [EmailAddress]
        [Remote("CheckIsEmailNotExist", "User", ErrorMessage = "This email is already used")]
        public string Email { get; set; }
        [Required]
        [Remote("CheckIsUsernameNotExist", "User", ErrorMessage = "This username is already used")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }

    }
}
