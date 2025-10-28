
namespace CrystalByRiya.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ImageGallery
    {
        [Key]
       
        public int Id { get; set; }

        [Required]        
        public string Productid { get; set; }

        [Required]
        public string Url { get; set; }
        
        [Required]
        public int Type { get; set; }

        [Required]
        public string AltText { get; set; }


    }
}
