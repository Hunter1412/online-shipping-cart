using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Models
{
    public class CartItem
    {
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public string? ImageName { get; set; }

    }
}