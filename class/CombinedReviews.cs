namespace CrystalByRiya.Models
{

    public class CombinedReview
    {
        public string ProductId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public string Email { get; set; }
        public string ReviewerName { get; set; }
        public DateTime? ReviewDate { get; set; }
        public bool IsApprovedReview { get; set; }
        public string ImageName { get; set; }
        public bool IsApprovedGallery { get; set; }

    }

}
