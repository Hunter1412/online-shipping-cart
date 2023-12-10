using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Models.DTOs;

namespace OnlineShoppingCart.Models.DTOs
{
    public class OrderDto
    {
        [Key]
        public string? Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        public string? OrderStatus { get; set; }

        public double OrderTotal { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }

        public string? ApprovedBy { get; set; }

        public string? ShippingId { get; set; }
        [ForeignKey("ShippingId")]
        public ShippingDto? Shipping { get; set; }

        public string? VoucherId { get; set; }
        [ForeignKey("VoucherId")]
        public VoucherDto? Voucher { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUserDto? AppUser { get; set; }

        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public OrderDto()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }

        public List<ProductDto>? Products { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}