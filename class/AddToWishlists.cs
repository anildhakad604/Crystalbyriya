namespace CrystalByRiya
{
    public class AddToWishlist
    {
        public int Qty { get; set; }
        public string ParentProductId
        {
            get; set;
        }
        public string ProductId
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }
        public string Image
        {
            get;
            set;
        }
        public string Price
        {
            get;
            set;
        }

        public double Gst
        {
            get;
            set;
        }

        public int Categoryid { get; set; }
        public int SubCategoryid { get; set; }

        public string MaterialName { get; set; }
        public string Size { get; set; }

    }
}
