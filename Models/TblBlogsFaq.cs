using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblBlogsFaq")]
public partial class TblBlogsFaq
{
    [Key]
    public int Id { get; set; }

    public int? BlogId { get; set; }

    [StringLength(255)]
    public string? Question { get; set; }

    public string? Answer { get; set; }
}
