using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineShoppingCart.Models.DTOs
{
    public class ContactDto
    {
        [Key]
        public Guid Id { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public string? Answer { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public AppUserDto? AppUser { get; set; }

        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public ContactDto()
        {
            CreateAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }
    }
}