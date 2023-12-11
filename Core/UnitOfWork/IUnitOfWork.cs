using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Core.Services;

namespace OnlineShoppingCart.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IImageRepository Images { get; }
        IInventoryRepository Inventory { get; }
        IContactRepository Contacts { get; }
        IFeedbackRepository Feedbacks { get; }

        IShippingRepository Shippings { get; }
        IVoucherRepository Vouchers { get; }
        IOrderDetailRepository OrderDetails { get; }
        IOrderRepository Orders { get; }
        ICartRepository Carts { get; }
        IPaypalServices PaypalServices { get; }
        Task CompleteAsync();
    }
}