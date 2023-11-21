using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Core.IRepository
{
    public class ICategoryRepository : IGenericRepository<Category>
    {
        public Task<bool> Add(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> All()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Category?> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Upsert(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}