using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblCart
{
    [Key]
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? UserEmail { get; set; }

    public string? Phone { get; set; }

    public string? ProductName { get; set; }

    public double? Price { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public string? Marketing { get; set; }

    [Column("material")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Material { get; set; }

    [Column("size")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Size { get; set; }

    [Column("addon")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Addon { get; set; }

    public int? Qty { get; set; }

    public double? Gst { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Image { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProductId { get; set; }
}
