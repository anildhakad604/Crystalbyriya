using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrystalByRiya.Models
{
    public static class Exist
    {
        public static int Exists(List<Item> cart, string id)
        {
            for (var i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
    public static class Existed
    {
        public static int Exists(List<AddToWishlist> cart, string id)
        {
            for (var i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
