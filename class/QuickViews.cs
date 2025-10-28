namespace CrystalByRiya
{
    public class QuickViews

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
        public List<string> Image
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
        public string Tags { get; set; }
        public int Categoryid { get; set; }
        public int SubCategoryid { get; set; }
        public string Description { get; set; }
        public List<string> MaterialName { get; set; }
        public List<string> Size { get; set; } 


        public List<string> AddOn { get; set; }
        
    }
}
