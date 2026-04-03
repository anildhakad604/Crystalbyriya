using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblReview
{
    [Key]
    public int Id { get; set; }

    public string Productid { get; set; } = null!;

    public string? Reviews { get; set; }

    public bool? IsApproved { get; set; }

    public int? Rating { get; set; }

    public string? Emailid { get; set; }

    public string? Name { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReviewDate { get; set; }
}
