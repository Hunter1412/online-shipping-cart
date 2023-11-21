using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Core.IRepository
{
    public class IProductRepository : IGenericRepository<Product>
    {
        public Task<bool> Add(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> All()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Upsert(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}