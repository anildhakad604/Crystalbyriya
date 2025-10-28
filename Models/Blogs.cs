using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class Blogs
    {
        [Key] 
        public int Blogid { get; set; }
        public string BlogTitle { get; set; }
        public string ShortDescription { get; set; }
        public string Blogdescription { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }        
        public string CustomUrl { get; set; }
        public string ThumbnailImage { get; set; }
        public string Keywords { get; set; }
        public bool IsActive { get; set; }
        public string? Alttext { get; set; }
        public string? Tags { get; set; }
        public string MetaTag { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
    }
}
