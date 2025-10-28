namespace CrystalByRiya.Models
{
    public class Employee
    {
        public int Id { get; set; } // Primary key
        public string Username { get; set; } // Username of the employee
        public string Email { get; set; } // Email of the employee
        public string Password { get; set; } // Password (hashed)
        public string ContactNo { get; set; } // Contact number
      public string Role { get; set; }
    }
}
