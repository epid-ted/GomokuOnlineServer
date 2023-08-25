using System.ComponentModel.DataAnnotations;

namespace LoginServer.Data.DTOs.Client
{
    public class RegisterRequestDto
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(6)]
        [MaxLength(12)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(6)]
        [MaxLength(12)]
        public string Password { get; set; }
    }
}
