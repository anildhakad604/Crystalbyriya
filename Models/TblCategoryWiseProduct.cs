using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblCategoryWiseProduct")]
public partial class TblCategoryWiseProduct
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string SkuCode { get; set; } = null!;

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }
}
