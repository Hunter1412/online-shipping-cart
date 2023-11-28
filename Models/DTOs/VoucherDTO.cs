using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Models.DTOs
{
    public class VoucherDto
    {
        [Key]
        [Required]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Code { get; set; }
        [Required]
        public double Discount { get; set; }
        [Required]
        public double MinimumBill { get; set; }
        [Required]
        public DateTime ExpDate { get; set; }

        public DateTime CreateAt { get; private set; }

        public VoucherDto()
        {
            CreateAt = DateTime.Now;
        }
    }
}