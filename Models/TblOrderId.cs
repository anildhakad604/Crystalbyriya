
namespace CrystalByRiya.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class TblOrderId
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Orderid { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public double CouponCodeapplied { get; set; }

        [Required]
        
        [DisplayName("Mobile")]
        public string Emailid { get; set; }

        [Required]
        public string Status { get; set; }
        
        [Required]
        public double TotalAmount { get; set; }
        public string PaymentFrom { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderNotes { get; set; }
    }
}
