using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Data.Entities
{
    public class Shipping
    {
        [Key, Required]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Wards { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public string? DeliveryType { get; set; }
        public double ShippingFee { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime EndAt { get; set; }


        public Shipping()
        {
            CreateAt = DateTime.Now;
        }
        public Order? Order { get; set; }

    }
}