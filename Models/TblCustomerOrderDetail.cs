using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblCustomerOrderDetail
{
    [Key]
    public int Id { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string OrderCode { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string SkuCode { get; set; } = null!;

    public int Qty { get; set; }

    public double Price { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public double? Gst { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Material { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Size { get; set; }

    [StringLength(255)]
    public string? ProductId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? AddOn { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }
}
