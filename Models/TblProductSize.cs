using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblProductSize
{
    [Key]
    [Column("SizeID")]
    public int SizeId { get; set; }

    [Column("ProductID")]
    [StringLength(50)]
    [Unicode(false)]
    public string ProductId { get; set; } = null!;

    [StringLength(50)]
    public string Size { get; set; } = null!;

    [Column("SKUCode")]
    [StringLength(50)]
    public string Skucode { get; set; } = null!;

    [Column("ImageURL")]
    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public int? StockQuantity { get; set; }

    public double? Price { get; set; }

    [StringLength(255)]
    public string? AltText { get; set; }
}
