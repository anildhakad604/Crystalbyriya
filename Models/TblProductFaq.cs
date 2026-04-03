using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblProductFaq")]
public partial class TblProductFaq
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ProductSku { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Question { get; set; } = null!;

    [Unicode(false)]
    public string Answer { get; set; } = null!;
}
