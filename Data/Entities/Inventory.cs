using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Data.Entities
{
    public class Inventory
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public DateTime DateAt { get; set; }
        [Required]
        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int? Quantity { get; set; }

        public string? Note { get; set; }


        public Inventory()
        {
            Id = Guid.NewGuid().ToString();
            DateAt = DateTime.Now;
        }
    }
}