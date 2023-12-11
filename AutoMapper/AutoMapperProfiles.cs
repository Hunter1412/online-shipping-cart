using AutoMapper;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;

namespace OnlineShoppingCart.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            CreateMap<ProductDto, Product>();
            CreateMap<Product, ProductDto>();

            CreateMap<ImageDto, Image>();
            CreateMap<Image, ImageDto>();


            CreateMap<InventoryDto, Inventory>();
            CreateMap<Inventory, InventoryDto>();

            CreateMap<VoucherDto, Voucher>();
            CreateMap<Voucher, VoucherDto>();

            CreateMap<OrderDto, Order>();
            CreateMap<Order, OrderDto>();

            CreateMap<OrderDetailDto, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailDto>();

            CreateMap<AppUserDto, AppUser>();
            CreateMap<AppUser, AppUserDto>();


            CreateMap<FeedbackDto, Feedback>();
            CreateMap<Feedback, FeedbackDto>();

            CreateMap<ContactDto, Contact>();
            CreateMap<Contact, ContactDto>();

            CreateMap<ShippingDto, Shipping>();
            CreateMap<Shipping, ShippingDto>();

            CreateMap<VoucherDto, Voucher>();
            CreateMap<Voucher, VoucherDto>();


        }
    }
}