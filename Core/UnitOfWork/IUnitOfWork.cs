using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Core.Repository;

namespace OnlineShoppingCart.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }

        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }

        IVoucherRepository Vouchers { get; }
        IImageRepository Images { get; }
        IInventoryRepository Inventory { get; }
        Task CompleteAsync();
    }
}