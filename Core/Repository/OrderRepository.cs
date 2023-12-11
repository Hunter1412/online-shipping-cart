using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Core.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<bool> Upsert(Order entity)
        {
            try
            {
                var existingOrder = await dbSet.SingleOrDefaultAsync(x => x.Id == entity.Id);
                if (existingOrder == null)
                {
                    return await Add(entity);
                }
                existingOrder.OrderStatus = entity.OrderStatus;
                existingOrder.PaymentMethod = entity.PaymentStatus;
                existingOrder.UpdateAt = DateTime.Now;
                existingOrder.ApprovedBy = entity.ApprovedBy;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert method error", typeof(VoucherRepository));
                return false;
            }
        }
    }
}