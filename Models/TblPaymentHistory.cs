using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblPaymentHistory
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    public string OrderId { get; set; } = null!;

    public double AmountRecieved { get; set; }

    public string Date { get; set; } = null!;
}
