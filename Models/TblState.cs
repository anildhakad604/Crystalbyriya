using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblState
{
    [Key]
    public int StateId { get; set; }

    [StringLength(150)]
    public string StateName { get; set; } = null!;
}
