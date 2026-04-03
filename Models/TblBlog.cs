using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrystalByRiya.Models;

public partial class TblBlog
{
    [Key]
    public int Blogid { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string BlogTitle { get; set; } = null!;

    [Unicode(false)]
    public string Blogdescription { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Author { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Category { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Image { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string CustomUrl { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string ThumbnailImage { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string Keywords { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateOnly? PublishedDate { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string ShortDescription { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? AltText { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Tags { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? MetaTag { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? MetaTitle { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? MetaDescription { get; set; }
}
