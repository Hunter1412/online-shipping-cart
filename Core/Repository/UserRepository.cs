using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Core.Repository
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context,ILogger logger) : base(context, logger) {  }

    }
}