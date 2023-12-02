using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingCart.Areas.ProductManage.Controllers
{
    [Area("ProductManage")]
    public class ProductController : Controller
    {
        [HttpGet("/admin/product")]
        public async Task<IActionResult> Index()
        {
            
            return View();
        }
    }
}