using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblAdminLogin")]
[Index("Username", Name = "UQ__TblAdmin__536C85E42F8A6FDE", IsUnique = true)]
public partial class TblAdminLogin
{
    [Key]
    public int Id { get; set; }

    [StringLength(256)]
    public string Username { get; set; } = null!;

    [StringLength(256)]
    public string PasswordHash { get; set; } = null!;
}
