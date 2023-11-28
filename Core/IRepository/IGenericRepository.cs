using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAll(string? includeProperties = null);
        Task<T?> Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<IEnumerable<T>> All();
        Task<T?> GetById(string id);
        Task<bool> Add(T entity);
        Task<bool> Upsert(T entity);
        Task<bool> Delete(string id);
    }

}