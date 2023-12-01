using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Models.DTOs
{
    public class FeedbackDto
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }

        public string? Image { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUserDto? AppUser { get; set; }

        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductDto? Product { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public FeedbackDto? Parent { get; set; }

        public List<FeedbackDto>? Children { get; set; }

        public DateTime CreateAt { get; private set; }
        public FeedbackDto()
        {
            CreateAt = DateTime.Now;
        }
    }
}