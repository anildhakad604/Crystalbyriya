using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblForgetPassword
{
    [Key]
    public int Id { get; set; }

    public int Uid { get; set; }

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [Column("datetime")]
    public DateTime Datetime { get; set; }
}
