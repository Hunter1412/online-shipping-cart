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

        public override async Task<bool> Upsert(Category entity)
        {
            try
            {
                var existingCate = await dbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
                if (existingCate == null)
                {
                    return await Add(entity);
                }
                existingCate.Name = entity.Name;
                existingCate.Slug = entity.Slug;
                existingCate.Description = entity.Description;
                existingCate.Image = entity.Image;
                existingCate.ParentId = entity.ParentId;
                existingCate.CreateAt = entity.CreateAt;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert method error", typeof(VoucherRepository));
                return false;
            }
        }


        public async Task<List<Category>> GetItemsSelectCategories(string id)
        {

            var query = (from c in _context.Categories select c)
                    .Include(c => c.Parent)
                    .Include(c => c.Children);

            var items = (await query.ToListAsync())
                            .Where(c => c.Parent == null)
                            .ToList();


            List<Category> resultItems = new() {
                new Category() {
                    Id = "-1",
                    Name = "Not parent category"
                }
            };
            Action<List<Category>, int> _ChangeTitleCategory = null!;

            void ChangeTitleCategory(List<Category> itemsDto, int level)
            {
                string prefix = string.Concat(Enumerable.Repeat("—", level));
                foreach (var item in itemsDto)
                {
                    resultItems.Add(new Category()
                    {
                        Id = item.Id,
                        Name = prefix + " " + item.Name + "_" + item.Id
                    });
                    if ((item.Id != id) && (item.Children != null) && (item.Children.Count > 0))
                    {
                        _ChangeTitleCategory(item.Children.ToList(), level + 1);
                    }
                }

            }

            _ChangeTitleCategory = ChangeTitleCategory;
            ChangeTitleCategory(items, 0);

            return resultItems;
        }

    }
}