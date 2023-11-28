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
            CreateMap<CategoryDTO, Category>();
            CreateMap<Category, CategoryDTO>();

            CreateMap<ProductDTO, Product>();
            CreateMap<ProductDTO, Product>();

            CreateMap<VoucherDTO, Voucher>();
            CreateMap<Voucher, VoucherDTO>();

            CreateMap<OrderDetailDTO, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailDTO>();

            CreateMap<AppUserDTO, AppUser>();
            CreateMap<AppUser, AppUserDTO>();
        }
    }
}