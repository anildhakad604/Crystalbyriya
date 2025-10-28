using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class Material
    {
        [Key]
        public int Id { get; set; }
        public string SkuCode { get; set; }
        public string MaterialName { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string AltText { get; set; }
    }
}
