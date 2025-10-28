using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Phone { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
        public string Marketing { get; set; }
        public string material { get; set; }
        public string size { get; set; }
        public string addon { get; set; }
        public int Qty { get; set; }
        public double Gst
        {
            get;
            set;
        }
        public string Image { get; set; }
    }
}
