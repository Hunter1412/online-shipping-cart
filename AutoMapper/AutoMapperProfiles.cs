using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            CreateMap<ProductDto, Product>();

            CreateMap<VoucherDto, Voucher>();
            CreateMap<Voucher, VoucherDto>();

            CreateMap<OrderDetailDto, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailDto>();

            CreateMap<AppUserDto, AppUser>();
            CreateMap<AppUser, AppUserDto>();
        }
    }
}