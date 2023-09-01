using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchServer.Web.Data.Entities
{
    [Table("Users")]
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(12)]
        public string Username { get; set; }

        [Required]
        public string EncodedPassword { get; set; }

        [Required]
        public DateTime LastStaminaUpdateTime { get; set; }

        [Required]
        [Range(0, 120)]
        public int Stamina { get; set; }
    }
}
