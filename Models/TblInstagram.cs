using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblInstagram")]
public partial class TblInstagram
{
    [Key]
    public int Id { get; set; }

    [Unicode(false)]
    public string InstagramUrl { get; set; } = null!;
}
