using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblWishlists")]
public partial class TblWishlist1
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Productid { get; set; }
}
