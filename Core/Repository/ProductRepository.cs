using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Core.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<bool> Upsert(Product entity)
        {
            try
            {
                var existingProduct = await dbSet.SingleOrDefaultAsync(x => x.Id == entity.Id);
                if (existingProduct == null)
                {
                    return await Add(entity);
                }
                existingProduct.Name = entity.Name;
                existingProduct.Slug = entity.Slug;
                existingProduct.Color = entity.Color;
                existingProduct.Size = entity.Size;
                existingProduct.Description = entity.Description;
                existingProduct.Price = entity.Price;
                existingProduct.Promotion = entity.Promotion;
                existingProduct.CreateAt = entity.CreateAt;
                existingProduct.CategoryId = entity.CategoryId;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert method error", typeof(VoucherRepository));
                return false;
            }
        }
    }
}