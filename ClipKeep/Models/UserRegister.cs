namespace ClipKeep.Models
{
    /// <summary>
    /// Hold data used to generate new users.
    /// </summary>
    public class UserRegister
    {
        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}