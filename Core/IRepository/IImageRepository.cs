using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingCart.Data;
using ImageEntity = OnlineShoppingCart.Data.Entities.Image;


namespace OnlineShoppingCart.Core.IRepository
{
    public interface IImageRepository : IGenericRepository<ImageEntity>
    {
    }
}