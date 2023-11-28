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

        public override async Task<IEnumerable<Voucher>> All()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(VoucherRepository));
                return new List<Voucher>();
            }
        }

        public override async Task<Voucher?> GetById(string id)
        {
            try
            {
                return await dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetById method error", typeof(VoucherRepository));
                return new Voucher();
            }
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


        public override async Task<bool> Delete(string id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    dbSet.Remove(exist);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete method error", typeof(VoucherRepository));
                return false;
            }
        }



    }
}