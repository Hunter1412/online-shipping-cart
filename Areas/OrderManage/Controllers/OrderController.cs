using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingCart.Areas.OrderManage.Controllers
{
    [Area("Order")]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/admin/order")]
        public IActionResult Index()
        {
            return View();
        }

    }
}