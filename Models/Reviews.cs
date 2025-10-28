namespace CrystalByRiya.Models
{
    public class TblReviews
    {
        public int Id { get; set; }
        public string Productid { get; set; }
        public string Reviews { get; set; }
        public bool IsApproved { get; set; }
        public int Rating { get; set; }
        public string Emailid { get; set; }
        public string Name { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}
