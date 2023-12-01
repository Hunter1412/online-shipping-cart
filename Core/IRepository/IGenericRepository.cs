using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>?> GetAll(string? includeProperties = null);
        Task<T?> Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<bool> Add(T entity);
        Task<bool> Upsert(T entity);
        bool Delete(T entity);
    }

}