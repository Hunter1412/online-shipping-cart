using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Models.DTOs
{
    public class InventoryDto
    {
        [Key]
        public string? Id { get; set; }
        public DateTime? DateAt { get; set; }
        [Required]
        [Display(Name = "Product Id")]
        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductDto? Product { get; set; }

        [Required]
        [Display(Name = "Input Quantity")]
        public int? Quantity { get; set; }

        [Required]
        public string? Note { get; set; }


        public InventoryDto()
        {
            DateAt = DateTime.Now;
        }
    }
}