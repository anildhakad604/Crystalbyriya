using CrystalByRiya.Pages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrystalByRiya.Models
{
    public class CategoryBanner
    {
        public int Id { get; set; }

        [Column("CategoryId")]
        [ForeignKey("TblCategory")]
        public int CategoryId { get; set; }

        public string BannerName { get; set; }

        public string BannerImage { get; set; }

        public virtual TblCategory TblCategory { get; set; }
    }
}
