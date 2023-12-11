using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Core.Repository;
using OnlineShoppingCart.Core.Services;
using OnlineShoppingCart.Data;

namespace OnlineShoppingCart.Core.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger _logger;
        public IUserRepository Users { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IImageRepository Images { get; private set; }
        public IInventoryRepository Inventory { get; private set; }
        public IContactRepository Contacts { get; private set; }
        public IFeedbackRepository Feedbacks { get; private set; }
        public IVoucherRepository Vouchers { get; private set; }
        public IShippingRepository Shippings { get; private set; }
        public IOrderDetailRepository OrderDetails { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public ICartRepository Carts { get; private set; }

        public IPaypalServices PaypalServices { get; private set; }
        private readonly IConfiguration _configuration;



        public UnitOfWork(ApplicationDbContext context, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logs");
            Users = new UserRepository(context, _logger);
            Categories = new CategoryRepository(context, _logger);
            Products = new ProductRepository(context, _logger);
            Images = new ImageRepository(context, _logger);
            Inventory = new InventoryRepository(context, _logger);
            Contacts = new ContactRepository(context, _logger);
            Feedbacks = new FeedbackRepository(context, _logger);
            Vouchers = new VoucherRepository(context, _logger);
            Shippings = new ShippingRepository(context, _logger);
            OrderDetails = new OrderDetailRepository(context, _logger);
            Orders = new OrderRepository(context, _logger);
            Carts = new CartRepository(context, _logger);

            _configuration = configuration;
            PaypalServices = new PaypalServices(_configuration);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}