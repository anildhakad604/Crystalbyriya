namespace CrystalByRiya.Models
{
    public class CategoryWiseProduct
    {
        public int Id { get; set; }
        public string SkuCode { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }
}