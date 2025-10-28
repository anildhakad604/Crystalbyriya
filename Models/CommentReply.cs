using System.ComponentModel.DataAnnotations;

namespace CrystalByRiya.Models
{
    public class CommentReply
    {
        [Key]
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Comment { get;set; }
        public bool IsApproved { get; set; }
        public DateTime Date { get; set; }


    }
}
