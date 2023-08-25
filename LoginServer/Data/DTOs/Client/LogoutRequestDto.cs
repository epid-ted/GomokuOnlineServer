using System.ComponentModel.DataAnnotations;

namespace LoginServer.Data.DTOs.Client
{
    public class LogoutRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string SessionId { get; set; }
    }
}
