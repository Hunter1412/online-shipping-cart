using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Data.Entities
{
    public class Cart
    {
        [Key, Required]
        public string? Id { get; set; }
        public int Quantity { get; set; }


        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }


        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public Cart()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }


    }
}