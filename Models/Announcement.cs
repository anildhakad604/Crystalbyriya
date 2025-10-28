using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }
        public string AnnouncementText { get; set; }
    }
}
