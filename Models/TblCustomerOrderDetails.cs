namespace CrystalByRiya.Models
{
    public class TblCustomerOrderDetails
    {
        public int Id { get; set; }
        public string OrderCode {  get; set; }
        public string SkuCode { get; set; }
        public int Qty { get; set; }

        public double Price {  get; set; }

        public string Status { get; set; }
        public double Gst {  get; set; }
        public string Material { get; set; }
        public string Size { get; set; }
        public string ProductId { get; set; }
        public string AddOn { get; set; }
        public string Email { get; set; }
    }
}
