using System.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineShoppingCart.Data.Entities
{
    public class Product
    {
        [Key,Required]
        [RegularExpression(@"\b(ST|OS|AS|CM)(\d{5})\b", ErrorMessage = "The Product-Id must be formatted as 'AAxxxxx', A is alphabet, x is the digits")]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Slug { get; set; }

        [DataType(DataType.Text)]
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }

        [Required]
        public double Price { get; set; }

        public double? Promotion { get; set; }

        public DateTime? CreateAt { get; set; }
        public Product()
        {
            CreateAt = DateTime.Now;
        }

        public string? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }


        public List<Image>? Images { get; set; }
        public List<Feedback>? Feedbacks { get; set; }
        public List<OrderDetail>? OrderDetail { get; set; }
        public List<Order>? Orders { get; set; }

        public List<Inventory>? Inventories { get; set; }

    }


}