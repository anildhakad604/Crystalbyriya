using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblBanner
{
    [Key]
    public int Id { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string MobileImageName { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string DesktopImageName { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string TargetUrl { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string AltText { get; set; } = null!;

    [StringLength(6)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Text { get; set; } = null!;
}
