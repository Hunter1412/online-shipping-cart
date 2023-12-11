using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;
using Org.BouncyCastle.Crypto.Digests;
using X.PagedList;

namespace OnlineShoppingCart.Controllers
{
    public class ViewOrderController : Controller
    {
        protected readonly ILogger<ViewOrderController> _logger;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public ViewOrderController(ILogger<ViewOrderController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index([FromQuery(Name = "page")] int? page, string? orderid = null)
        {

            var orders = await _unitOfWork.Orders.GetAll("OrderDetails");
            if (orders == null)
            {
                return View(new List<OrderDto>());
            }

            var orderDtoList = orders.Select(o => _mapper.Map<OrderDto>(o)).OrderByDescending(x => x.CreateAt).ToList();
            if (orderid != null)
            {
                orderDtoList = orderDtoList.Where(o => o.Id.Contains(orderid)).ToList();
                if (orderDtoList.Count > 0)
                {
                    TempData["success"] = $"Search order with {orderid}";
                }
                else
                {
                    TempData["error"] = $"Not found order with {orderid}";
                }
            }

            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            ViewBag.CountOrder = orders.Count;

            return View(orderDtoList.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder([FromForm] string orderId)
        {
            _logger.LogInformation("CancelOrder action");
            var order = await _unitOfWork.Orders.Get(x => x.Id == orderId);
            if (order != null)
            {
                order.OrderStatus = "Cancel";
                await _unitOfWork.CompleteAsync();
                TempData["success"] = $"Cancel order {orderId} successfully!";
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Search(string orderId)
        {
            _logger.LogInformation("CancelOrder action");
            var orderList = await _unitOfWork.Orders.GetAll("OrderDetails");
            var result = orderList?.Where(x => x.Id!.Contains(orderId)).ToList();

            if (result != null)
            {
                TempData["success"] = $"Search order {orderId} successfully!";
                return RedirectToAction(nameof(Index), new { data = result });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}