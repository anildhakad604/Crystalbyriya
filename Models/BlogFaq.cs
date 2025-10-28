namespace CrystalByRiya.Models
{
    public class BlogFaq
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
