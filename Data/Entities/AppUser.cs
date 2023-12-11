using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineShoppingCart.Data.Entities
{
    public class AppUser : IdentityUser
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

        public DateTime? CreateAt { get; private set; }
        public AppUser()
        {
            CreateAt = DateTime.Now;
        }


        [ValidateNever]
        public List<Order>? Orders { get; set; }
        [ValidateNever]
        public List<Feedback>? Feedbacks { get; set; }
        [ValidateNever]
        public List<Contact>? Contacts { get; set; }


    }
}