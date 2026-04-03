using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class Coupon
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string CouponCode { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int MaxUses { get; set; }

    public int MaxUsesPerUser { get; set; }

    [StringLength(20)]
    public string Type { get; set; } = null!;

    public double DiscountAmount { get; set; }

    public double? MinAmount { get; set; }

    [StringLength(10)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime StartsAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }
}
