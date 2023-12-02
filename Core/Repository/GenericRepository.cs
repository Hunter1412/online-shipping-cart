using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Data;

namespace OnlineShoppingCart.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> dbSet;
        protected readonly ILogger _logger;
        public GenericRepository(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            dbSet = context.Set<T>();
        }

        public virtual async Task<List<T>?> GetAll(string? includeProperties = null)
        {
            try
            {
                IQueryable<T> query = dbSet;
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProp in includeProperties
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return await query.ToListAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll method error", typeof(GenericRepository<T>));
                return null;
            }
        }

        public virtual async Task<T?> Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            try
            {
                IQueryable<T> query = dbSet;
                query = query.Where(filter);
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProp in includeProperties
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return await query.FirstOrDefaultAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "{Repo} Get method error", typeof(GenericRepository<T>));
                return null;
            }

        }


        public virtual async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual bool Delete(T entity)
        {
            try
            {
                dbSet.Remove(entity);
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete method error", typeof(GenericRepository<T>));
                return false;
            }
        }
    }
}