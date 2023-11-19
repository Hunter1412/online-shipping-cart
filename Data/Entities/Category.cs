using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineShoppingCart.Data.Entities
{
    public class Category
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

        public string? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Category? Parent { get; set; }
        public List<Category>? Children { get; set; }

        public DateTime CreateAt { get; private set; }
        public Category()
        {
            CreateAt = DateTime.Now;
        }

        [ValidateNever]
        public List<Product>? Products { get; set; }




    }
}