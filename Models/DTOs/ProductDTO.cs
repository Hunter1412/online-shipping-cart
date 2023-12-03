using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineShoppingCart.Models.DTOs
{
    public class ProductDto
    {
        [Key, Required]
        [RegularExpression(@"\b(ST|OS|AS|CM)(\d{5})\b", ErrorMessage = "The Product-Id must be formatted as 'AAxxxxx', A is alphabet, x is the digits")]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public string? Slug { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string? Description { get; set; }
        [Required]
        public string? Size { get; set; }

        [Required]
        public string? Color { get; set; }

        [Required]
        public double Price { get; set; }

        public double? Promotion { get; set; }

        public DateTime? CreateAt { get; set; }
        public ProductDto()
        {
            CreateAt = DateTime.Now;
        }
        [Display(Name = "Category")]
        public string? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public CategoryDto? Category { get; set; }

        public List<ImageDto>? Images { get; set; }

        // [Required]
        [Display(Name = "Click here to upload product image")]
        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }

        public List<FeedbackDto>? Feedbacks { get; set; }
        public List<OrderDetailDto>? OrderDetails { get; set; }
        public List<OrderDto>? Orders { get; set; }

        [ValidateNever]
        public List<InventoryDto>? Inventories { get; set; }

        [NotMapped]
        public int? Quantity { get; set; }
    }
}