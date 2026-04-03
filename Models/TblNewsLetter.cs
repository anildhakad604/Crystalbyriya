using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblNewsLetter
{
    [Key]
    public int Id { get; set; }

    public string NewsletterEmail { get; set; } = null!;
}
