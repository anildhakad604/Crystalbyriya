using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblIngredient
{
    [StringLength(255)]
    public string Skucode { get; set; } = null!;

    public string? IngredientsName { get; set; }

    [Key]
    public int Id { get; set; }
}
