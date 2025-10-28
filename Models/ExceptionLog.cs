namespace CrystalByRiya.Models
{
    public class ExceptionLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string PageName { get; set; } 
    }
}
