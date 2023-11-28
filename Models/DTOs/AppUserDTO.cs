using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace OnlineShoppingCart.Models.DTOs
{
    public class AppUserDto : IdentityUser
    {
        [PersonalData]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [PersonalData]
        [StringLength(100)]
        public string? LastName { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [PersonalData]
        public string? Avatar { get; set; }

        [PersonalData]
        public bool Gender { get; set; }

        public DateTime? LastAt { get; set; }

        [ValidateNever]
        public List<OrderDto>? Orders { get; set; }

        [ValidateNever]
        public List<FeedbackDto>? Feedbacks { get; set; }

        [ValidateNever]
        public List<ContactDto>? Contacts { get; set; }

        [ValidateNever]
        public List<CartDto>? Carts { get; set; }

        public AppUserDto()
        {
            LastAt = DateTime.Now;
        }
    }
}