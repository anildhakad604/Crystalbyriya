using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }
        public string Email {  get; set; }
        public string Name { get; set; }
        public int Phonenumber { get; set; }
        public string Message { get; set; }
    }
}
