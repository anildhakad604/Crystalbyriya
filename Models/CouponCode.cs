using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class CouponCode
{
    [Key]
    public int Id { get; set; }

    [Column("CouponCode")]
    public string CouponCode1 { get; set; } = null!;

    public string CouponName { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ApplicableFrom { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ApplicableTo { get; set; }

    public bool IsActive { get; set; }

    public int DiscountPercentage { get; set; }
}
