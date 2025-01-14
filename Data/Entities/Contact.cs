using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineShoppingCart.Data.Entities
{
    public class Contact
    {
        [Key, Required]
        public string? Id { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public string? Answer { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public Contact()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }


    }
}