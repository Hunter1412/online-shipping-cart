using OnlineShoppingCart.Core.IRepository;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingCart.Core.Repository
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {

        public VoucherRepository(ApplicationDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<bool> Upsert(Voucher entity)
        {
            try
            {
                var existingUser = await dbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
                if (existingUser == null)
                {
                    return await Add(entity);
                }
                existingUser.Name = entity.Name;
                existingUser.Code = entity.Code;
                existingUser.Discount = entity.Discount;
                existingUser.MinimumBill = entity.MinimumBill;
                existingUser.ExpDate = entity.ExpDate;

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