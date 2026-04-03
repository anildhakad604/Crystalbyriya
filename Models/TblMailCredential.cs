using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblMailCredential
{
    [Key]
    public int Id { get; set; }

    [Column("MailID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MailId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? MailPassword { get; set; }
}
