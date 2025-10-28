using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class NewsLetter
    {
        [Key]
        public int Id { get; set; }
        public string NewsletterEmail { get; set; }
    }
}
