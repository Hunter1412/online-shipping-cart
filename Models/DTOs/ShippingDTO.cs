using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Models.DTOs
{
    public class ShippingDto
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Province { get; set; }
        [Required]
        public string? Wards { get; set; }
        [Required]
        public string? Address { get; set; }
        public string? Note { get; set; }
        public string? DeliveryType { get; set; }

        public DateTime CreateAt { get; private set; }
        public ShippingDto()
        {
            CreateAt = DateTime.Now;
        }
    }
}