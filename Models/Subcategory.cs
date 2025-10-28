namespace CrystalByRiya.Models
{
    public class Subcategory
    {
        public int SubCategoryid { get; set; }       
        public string SubCategoryname { get; set; }
        public string Categoryimage { get; set; }
        public int CategoryId { get; set; }
        public string ? CategoryDescription { get;set; }
        public string ? ThumbnailImage { get;set; }
        public string MetaTag { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
    }
}
