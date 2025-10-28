using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class AddOn
    {
        [Key]
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string AddOnName { get; set; }
        public double Price { get; set; }
    }
}
