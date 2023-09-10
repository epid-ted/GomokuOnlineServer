namespace LoginServer.Data.DTOs
{
    public class KickoutRequestDto
    {
        public int UserId { get; set; }
        public string ServerName { get; set; }
        public string ServerSessionId { get; set; }
    }
}
