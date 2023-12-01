using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineShoppingCart.Models.DTOs
{
    public class CategoryDto
    {
        [Key]
        [Required]
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Slug { get; set; }
        [DataType(DataType.Text)]
        public string? Description { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }

        public string? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public CategoryDto? Parent { get; set; }
        public List<CategoryDto>? Children { get; set; }

        public DateTime CreateAt { get; set; }
        public CategoryDto()
        {
            CreateAt = DateTime.Now;
        }

        [ValidateNever]
        public List<ProductDto>? Products { get; set; }
    }
}