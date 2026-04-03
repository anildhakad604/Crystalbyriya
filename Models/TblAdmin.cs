using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblAdmin")]
public partial class TblAdmin
{
    [StringLength(50)]
    public string EmailId { get; set; } = null!;

    [StringLength(50)]
    public string Password { get; set; } = null!;

    [Key]
    public int Id { get; set; }
}
