using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblCouponEmailUsed
{
    [Key]
    public int Id { get; set; }

    public string CouponCode { get; set; } = null!;

    public string Emailid { get; set; } = null!;
}
