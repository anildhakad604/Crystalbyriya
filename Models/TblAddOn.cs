using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblAddOn")]
public partial class TblAddOn
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProductId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? AddOnName { get; set; }

    public double? Price { get; set; }
}
