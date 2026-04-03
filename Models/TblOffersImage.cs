using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblOffersImage
{
    [Key]
    public int Id { get; set; }

    public string Image { get; set; } = null!;

    public bool Enable { get; set; }

    [StringLength(500)]
    public string? Url { get; set; }
}
