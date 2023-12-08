using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MailKit;

namespace OnlineShoppingCart.Models
{
    public class FormCheckOut
    {
        [Required(ErrorMessage = "Please enter your name")]
        [MinLength(3)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
        ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [Phone]
        public string? Phone { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? District { get; set; }
        [Required]
        public string? Ward { get; set; }
        [Required]
        public string? Address { get; set; }
        public string? Note { get; set; }
        public string? DeliveryType { get; set; }
        public double? ShippingFee { get; set; }

        public string? Code { get; set; }

        public string PaymentMethod { get; set; } = "PayPal";

    }
}