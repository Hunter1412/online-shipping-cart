using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Models.DTOs
{
    public class ImageDto
    {
        [Key]
        public string? Id { get; set; }
        public string? ImageName { get; set; }
        public string? ImagePath { get; set; }

        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductDto? Product { get; set; }


    }
}