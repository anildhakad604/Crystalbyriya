using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblCommentReply")]
public partial class TblCommentReply
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    public int BlogId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public bool? IsApproved { get; set; }
}
