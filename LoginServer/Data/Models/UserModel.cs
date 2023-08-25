namespace LoginServer.Data.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string EncodedPassword { get; set; }
    }
}
