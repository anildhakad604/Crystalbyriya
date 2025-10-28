using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class ProductSizes
    {
        [Key]
            public int SizeID { get; set; }
            public string ProductID { get; set; }
            public string Size { get; set; }
            public string SKUCode { get; set; }
            public string? ImageURL { get; set; }
            public int StockQuantity { get; set; }
            public double Price { get; set; }
        public string AltText { get; set; }

    }
}
