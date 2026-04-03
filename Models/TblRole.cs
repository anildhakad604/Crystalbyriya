using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblRole
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Role { get; set; } = null!;
}
