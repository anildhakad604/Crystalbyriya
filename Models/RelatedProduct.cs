using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class RelatedProduct
    {
        [Key]
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Skucode{ get; set; }
    }
}
