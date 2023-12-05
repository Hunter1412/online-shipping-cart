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
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context, ILogger logger) : base(context, logger)
        {
        }


        public override async Task<bool> Upsert(Contact entity)
        {
            try
            {
                var existingContact = await dbSet.SingleOrDefaultAsync(x => x.Id == entity.Id);
                if (existingContact == null)
                {
                    return await Add(entity);
                }
                existingContact.Subject = entity.Subject;
                existingContact.Content = entity.Content;
                existingContact.Answer = entity.Answer;
                existingContact.UserId = entity.UserId;
                existingContact.CreateAt = entity.CreateAt;
                existingContact.UpdateAt = entity.UpdateAt;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert method error", typeof(ContactRepository));
                return false;
            }
        }
    }
}