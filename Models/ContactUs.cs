using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email {  get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phonenumber { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
