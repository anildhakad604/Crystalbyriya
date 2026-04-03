using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblProfile")]
public partial class TblProfile
{
    [Key]
    public int Id { get; set; }

    public string Emailid { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    [StringLength(50)]
    public string State { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string PinCode { get; set; } = null!;

    [StringLength(50)]
    public string? Gst { get; set; }

    [StringLength(150)]
    public string? Apartment { get; set; }

    [StringLength(150)]
    public string LastName { get; set; } = null!;
}
