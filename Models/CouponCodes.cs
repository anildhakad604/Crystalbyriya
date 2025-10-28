using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class CouponCodes
    {
        [Key]
        public int Id { get; set; }

        public string CouponCode { get; set; }

        public string CouponName { get; set; }

        public string Description { get; set; }

        public DateTime ApplicableFrom { get; set; }

        public DateTime ApplicableTo { get; set; }

        public bool IsActive { get; set; }
        public int DiscountPercentage { get; set; }
    }
}
