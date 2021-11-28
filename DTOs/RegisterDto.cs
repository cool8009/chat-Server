using System.ComponentModel.DataAnnotations;

namespace ChatService
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}