using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchServer.Web.Data.Entities
{
    [Table("MatchResults")]
    public class MatchResult
    {
        [Key]
        public int MatchId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Result { get; set; }
        public int? User1Id { get; set; }
        public User User1 { get; set; }
        public int? User2Id { get; set; }
        public User User2 { get; set; }
    }
}
