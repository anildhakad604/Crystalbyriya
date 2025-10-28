namespace CrystalByRiya.StoredProcedure
{
    public class SPSubCategoryWiseProduct
    {
        public string SkuCode { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public string Price { get; set; }
        public string ParentUrl { get; set; }
        public string Thumbnail { get; set; }
        public string ParentCode { get; set; }
        public DateTime AddedOn { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Url { get; set; }
        public string SubCategoryname { get; set; }
    }
}
