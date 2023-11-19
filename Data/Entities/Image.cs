using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Data.Entities
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; }
        public string? ImageName { get; set; }
        public string? ImagePath { get; set; }

        public string? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }


        public Guid? FeedbackId { get; set; }
        [ForeignKey("FeedbackId")]
        public Feedback? Feedback { get; set; }


    }
}