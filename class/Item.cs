using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrystalByRiya.Models
{
    public class Item
    {
       
        public int Qty { get; set; }
        public string ProductId
        {
            get; set;
        }
        public string skucode
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }
        public string Image
        {
            get;
            set;
        }
        public double Price
        {
            get;
            set;
        }

        public double Gst
        {
            get;
            set;
        }

        public int Categoryid { get; set; }
        public int SubCategoryid { get; set; }

        public string MaterialName { get; set; } = "NA";
        public string Size { get; set; } = "NA";
        public string Addon { get; set; } = "NA";
      
    }
}
