using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Data.Entities
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }

        public string? Image { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Feedback? Parent { get; set; }

        public List<Feedback>? Children { get; set; }

        public DateTime CreateAt { get; private set; }
        public Feedback()
        {
            CreateAt = DateTime.Now;
        }

    }
}