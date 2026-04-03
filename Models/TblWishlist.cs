using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblWishlist")]
public partial class TblWishlist
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserEmail { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProductName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Price { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Marketing { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Image { get; set; }

    [Column("skucode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Skucode { get; set; }
}
