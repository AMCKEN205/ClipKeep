using System.ComponentModel.DataAnnotations;
using System.Web;
using ClipKeep.Models.CustomModelValidators;
using Microsoft.Owin.Security;


namespace ClipKeep.Models
{
    /// <summary>
    /// Hold data pertaining to individual users for log in authentication.
    /// </summary>
    public class UserLogin
    {
        /// <summary>
        /// The useranme provided by the user
        /// </summary>
        [Required(ErrorMessage = "Enter your username.")]
        [MaxLength(15, ErrorMessage = "Usernames shouldn't be longer than 15 characters.")]
        [MinLength(3, ErrorMessage = "Usernames shouldn't be shorter than 3 characters.")]
        public string Username { get; set; }

        /// <summary>
        /// The password provided by the user
        /// </summary>
        [Required(ErrorMessage = "Enter your password.")]
        [MaxLength(20, ErrorMessage = "Passwords shouldn't be longer than 20 characters.")]
        [MinLength(6, ErrorMessage = "Passwords shouldn't be shorter than 6 characters.")]
        [AuthenticateUser(nameof(Username), ErrorMessage = "The given password doesn't match the username entered.")]
        public string Password { get; set; }

        /// <summary>
        /// The PassSalt asscoiated with a stored user. Used for comparing the provided
        /// password's hash to the stored hash for the identified user's username on
        /// stored user JSON deserialization.
        /// </summary>
        public string PassSalt { get; set; }


    }
}