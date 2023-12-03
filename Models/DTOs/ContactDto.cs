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
        public string? Id { get; set; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string? Content { get; set; }
        public string? Answer { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
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