using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OnlineShoppingCart.Areas.CategoryManage.Controllers
{
    [Area("CategoryManage")]
    public class CategoryController : Controller
    {
        //test
        [HttpGet("/admin/category")]
        public IActionResult Index()
        {
            return View();
        }
    }
}