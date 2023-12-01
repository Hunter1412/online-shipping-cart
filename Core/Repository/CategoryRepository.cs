using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Core.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context, ILogger logger) : base(context, logger)
        {
        }
        public override async Task<IEnumerable<Category>> All()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(VoucherRepository));
                return new List<Category>();
            }
        }
        public override async Task<Category?> GetById(string id)
        {
            try
            {
                return await dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetById method error", typeof(VoucherRepository));
                return new Category();
            }
        }


        public override async Task<bool> Delete(string id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    dbSet.Remove(exist);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete method error", typeof(VoucherRepository));
                return false;
            }
        }
    }
}