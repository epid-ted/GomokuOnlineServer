using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginServer.Data.Entities
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
    }
}
