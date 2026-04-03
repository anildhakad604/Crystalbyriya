using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblEmployee")]
public partial class TblEmployee
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(15)]
    public string ContactNo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Role { get; set; } = null!;
}
