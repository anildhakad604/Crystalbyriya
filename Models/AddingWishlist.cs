using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class AddingWishlist
    {
        [Key]
        public int Id { get; set; }
         public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Phone { get; set; }
        public string ProductName { get; set; }
        
        public string Image { get; set; }
        public string Price { get; set; }
        public DateTime Date { get; set; }
        public string Marketing { get; set; }
        public string skucode { get; set; }
    }
}
