using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblImageGallery
{
    [Column("ProductID")]
    [StringLength(255)]
    public string ProductId { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int? Type { get; set; }

    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string? AltText { get; set; }
}
