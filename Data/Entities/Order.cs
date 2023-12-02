using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OnlineShoppingCart.Data.Entities
{
    public class Order
    {
        [Key, Required]
        public string? Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        public string? OrderStatus { get; set; }

        public double OrderTotal { get; set; }

        public int PaymentStatus { get; set; }
        public int PaymentMethod { get; set; }

        public string? ApprovedBy { get; set; }

        public string? ShippingId { get; set; }
        [ForeignKey("ShippingId")]
        public Shipping? Shipping { get; set; }

        public string? VoucherId { get; set; }
        [ForeignKey("VoucherId")]
        public Voucher? Voucher { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public Order()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }

        public List<Product>? Products { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }


    }
}