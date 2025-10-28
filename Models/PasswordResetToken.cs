namespace CrystalByRiya.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public Guid ResetToken { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

}
