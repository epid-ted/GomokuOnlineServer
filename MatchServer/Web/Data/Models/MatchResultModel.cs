namespace MatchServer.Web.Data.Models
{
    public class MatchResultModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Result { get; set; }
        public int[] UserIds { get; set; }
        public string[] Usernames { get; set; }
    }
}
