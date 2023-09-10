using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginServer.Data.Entities
{
    [Table("UserStamina")]
    public class UserStamina
    {
        public int UserStaminaId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }
        [Required]
        [Range(0, 120)]
        public int Stamina { get; set; }
    }
}
