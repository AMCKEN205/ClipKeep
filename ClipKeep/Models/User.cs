using System.ComponentModel.DataAnnotations;
using ClipKeep.Models.CustomModelValidators;
using ClipKeep.Models.Interfaces;

namespace ClipKeep.Models
{
    /// <summary>
    /// Hold data pertaining to individual users.
    /// </summary>
    public class User
    {

        [Required(ErrorMessage = "Enter your username.")]
        [MaxLength(15, ErrorMessage = "Usernames shouldn't be longer than 15 characters.")]
        [MinLength(3, ErrorMessage = "Usernames shouldn't be shorter than 3 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Enter your password.")]
        [MaxLength(20, ErrorMessage = "Passwords shouldn't be longer than 20 characters.")]
        [MinLength(6, ErrorMessage = "Passwords shouldn't be shorter than 6 characters.")]
        [UnameMatchesPass(nameof(Username), ErrorMessage = "The given password doesn't match the username entered.")]
        public string Password { get; set; }

        public string PassSalt { get; set; }



    }
}