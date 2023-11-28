using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingCart.Areas.DashBoard.Controllers
{
    [Area("Dashboard")]
    public class DashBoardController : Controller
    {
        [HttpGet("/admin/dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}