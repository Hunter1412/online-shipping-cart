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
        [RegularExpression(@"\b(CP)(\d{5})\b", ErrorMessage = "The Id must be formatted as 'CPxxxxx', x is the digits")]
        public string? Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }
        public string? Slug { get; set; }
        [DataType(DataType.Text)]
        public string? Description { get; set; }
        public string? Image { get; set; }
        [Display(Name = "Click here to upload image file")]
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


        public void ChildrenCategoryIDs(ICollection<CategoryDto> childcates, List<string> ids)
        {
            if (childcates == null)
            {
                childcates = this.Children;
            }
            foreach (CategoryDto item in childcates)
            {
                ids.Add(item.Id);
                ChildrenCategoryIDs(item.Children, ids);
            }
        }
    }
}