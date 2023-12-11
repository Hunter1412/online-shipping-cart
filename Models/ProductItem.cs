using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Models.DTOs;

namespace OnlineShoppingCart.Models
{
    public class ProductItem
    {
        public ProductItem(ProductDto productDto)
        {
            Id = productDto.Id;
            Name = productDto.Name;
            Slug = productDto.Slug;
            Description = productDto.Description;
            Size = productDto.Size;
            Color = productDto.Color;
            Price = productDto.Price;
            Promotion = (double)productDto.Promotion! > 0 ? (double)productDto.Promotion : 0.00;
            Stock = productDto.Inventories?.Select(x => x.Quantity).Sum();
            ImageNameFirst = productDto.Images?.Select(x => x.ImageName).FirstOrDefault() ?? "product_velvet-finish-notebook-a5.webp";
            Images = productDto.Images;
            Feedbacks = productDto.Feedbacks;
            OrderDetails = productDto.OrderDetails;
            Orders = productDto.Orders;

            CategoryId = productDto.CategoryId;

            NewPrice = (double)(productDto.Price - productDto.Promotion);
            Save = (double)(productDto.Promotion / productDto.Price);

            CountReview = productDto.Feedbacks!.Select(f => f.Id).Count();

            AverageRating = CountReview > 0
                    ? productDto.Feedbacks!.Where(f => f.ParentId == null).Select(r => r.Rating).Average()
                    : 0;
        }

        public string? Id { get; set; }

        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }

        public string? Size { get; set; }
        public string? Color { get; set; }
        public double Price { get; set; }
        public double Promotion { get; set; } = 0.00;
        public int? Stock { get; set; }

        public string? ImageNameFirst { get; set; }

        public List<ImageDto>? Images { get; set; }
        public List<FeedbackDto>? Feedbacks { get; set; }

        public List<OrderDetailDto>? OrderDetails { get; set; }
        public List<OrderDto>? Orders { get; set; }

        public double Save { get; set; }
        public double NewPrice { get; set; }

        public double AverageRating { get; set; } = 0.00;
        public int CountReview { get; set; } = 0;



        public string? CategoryId { get; set; }







    }
}