namespace ClipKeep.Models
{
    /// <summary>
    /// Hold data pertaining to individual users.
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}