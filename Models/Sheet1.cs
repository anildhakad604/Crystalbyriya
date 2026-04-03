using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Keyless]
[Table("Sheet1$")]
public partial class Sheet1
{
    public double? Id { get; set; }

    public double? BlogId { get; set; }

    [StringLength(255)]
    public string? Question { get; set; }

    public string? Answer { get; set; }

    [StringLength(255)]
    public string? F5 { get; set; }
}
