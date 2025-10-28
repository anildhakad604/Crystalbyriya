using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string SkuCode { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public string Price { get; set; }
        public string Keywords { get; set; }
        public string ProductDescription { get; set; }
        
        public string AltText { get; set; }
        public string ParentUrl { get; set; }
        public string Thumbnail { get; set; }
        public string ParentCode { get; set; }         
        public string ShortDescription { get; set; }
        public string Tags { get; set; }
        public DateTime AddedOn { get; set; }
        public string? Size { get; set; } = null;
        public string MetaTag { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
    }
}
