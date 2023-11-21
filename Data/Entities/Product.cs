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
        [Key]
        [Required]
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

        [Required]
        public int? InputQuantity { get; set; }

        public int? InventoryQuantity { get; set; }

        public DateTime? CreateAt { get; private set; }
        public Product()
        {
            CreateAt = DateTime.Now;
        }

        public string? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }


        [ValidateNever]
        public List<Image>? Images { get; set; }
        [ValidateNever]
        public List<OrderDetail>? OrderDetail { get; set; }

        [ValidateNever]
        public List<Feedback>? Feedbacks { get; set; }




    }


}