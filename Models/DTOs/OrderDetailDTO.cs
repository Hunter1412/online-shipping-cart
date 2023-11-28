using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Models.DTOs
{
    public class OrderDetailDto
    {
        [Key]
        [Required]
        public string? Id { get; set; }

        public string? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OrderDto? Order { get; set; }

        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductDto? Product { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}