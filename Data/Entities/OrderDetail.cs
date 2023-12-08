using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Data.Entities
{
    public class OrderDetail
    {
        [Required]
        public string? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        [Required]
        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }

        [Required]
        [StringLength(16)]
        public string? OrderNumber { get; set; }



    }
}