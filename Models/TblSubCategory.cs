using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

[Table("TblSubCategory")]
[Index("SubCategoryname", Name = "IX_TblSubCategory_SubCategoryname")]
public partial class TblSubCategory
{
    [Key]
    public int SubCategoryid { get; set; }

    [StringLength(255)]
    public string SubCategoryname { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string Categoryimage { get; set; } = null!;

    public int? CategoryId { get; set; }

    [Unicode(false)]
    public string? CategoryDescription { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? ThumbnailImage { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? MetaTitle { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? MetaDescription { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? MetaTag { get; set; }
}
