using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class MailCredentials
    {
        [Key]
        public int Id { get; set; }
        public string MailId { get; set; }

        public string MailPassword { get; set; }

    }
}
