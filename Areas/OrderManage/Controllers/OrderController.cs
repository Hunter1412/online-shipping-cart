using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Models.DTOs;

namespace OnlineShoppingCart.Areas.OrderManage.Controllers
{
    [Area("OrderManage")]
    [Authorize(Roles = "admin,employee")]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<AppUser> _userManager;

        public OrderController(ILogger<OrderController> logger, IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("/admin/order")]
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.Orders.GetAll();
            if (orders == null)
            {
                return View(new List<OrderDto>());
            }
            var orderDtoList = orders.Select(o => _mapper.Map<OrderDto>(o)).OrderByDescending(x => x.CreateAt).ToList();
            return View(orderDtoList);
        }

        [HttpGet("/admin/order/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var order = await _unitOfWork.Orders.Get(o => o.Id == id, "OrderDetails,Shipping");
            if (order == null)
            {
                return View(new OrderDto());
            }
            return View(_mapper.Map<OrderDto>(order));
        }


        [HttpGet("/admin/delivery-report")]
        public async Task<IActionResult> Delivery()
        {
            var shippingList = await _unitOfWork.Shippings.GetAll("Order");
            if (shippingList == null)
            {
                return View(new List<ShippingDto>());
            }
            var shippingDtoList = shippingList.Select(o => _mapper.Map<ShippingDto>(o))
                                    .OrderByDescending(x => x.CreateAt)
                                    .ToList();
            return View(shippingDtoList);
        }


        [HttpGet("/admin/delivery-details")]
        public async Task<IActionResult> DeliveryDetails(string id)
        {
            var order = await _unitOfWork.Orders.Get(o => o.ShippingId == id, "OrderDetails,Shipping");
            if (order == null)
            {
                return View(new OrderDto());
            }
            return View(_mapper.Map<OrderDto>(order));
        }

        public List<ViewOrderStatusModel> GetOrderStatusList()
        {
            return new List<ViewOrderStatusModel>{
                new() { Id = "Pending" , Name = "Pending"},
                new() { Id = "Confirm" , Name = "Confirm"},
                new() { Id = "Delivering" , Name = "Delivering"},
                new() { Id = "Complete" , Name = "Complete"},
                new() { Id = "Cancel" , Name = "Cancel"},
                new() { Id = "Await_Return" , Name = "Await_Return"},
                new() { Id = "Returned" , Name = "Returned"}
            };

        }

        public List<ViewPaymentStatusModel> GetPaymentStatusList()
        {
            return new List<ViewPaymentStatusModel>{
                new() { Id = "Await_Payment" , Name = "Await_Payment"},
                new() { Id = "Finish" , Name = "Finish"},
                new() { Id = "Refund" , Name = "Refund"}
            };
        }

        [HttpGet("/admin/order/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var order = await _unitOfWork.Orders.Get(p => p.Id == id);
            if (order == null)
            {
                _logger.LogError($"The product with the {id} doesn't exist");
                return NotFound();
            }

            ViewBag.OrderStatus = new SelectList(GetOrderStatusList(), "Id", "Name", order.OrderStatus);
            ViewBag.PaymentStatus = new SelectList(GetPaymentStatusList(), "Id", "Name", order.PaymentStatus);

            return View(_mapper.Map<OrderDto>(order));
        }

        [HttpPost("/admin/order/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                TempData["error"] = "Order ID not found.";
                return RedirectToAction(nameof(Edit));
            }
            // if (!ModelState.IsValid)
            // {
            //     return RedirectToAction(nameof(Edit));
            // }
            try
            {
                AppUser? user = await _userManager.GetUserAsync(User);
                var order = _mapper.Map<Order>(orderDto);
                order.ApprovedBy = user?.Email;
                await _unitOfWork.Orders.Upsert(order);
                await _unitOfWork.CompleteAsync();

                TempData["success"] = "Update order status success";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error edit method");
                return RedirectToAction(nameof(Edit));
            }
        }

        [HttpGet("/admin/payment")]
        public async Task<IActionResult> Payment()
        {
            var order = await _unitOfWork.Orders.GetAll("Shipping");
            if (order == null)
            {
                return View(new OrderDto());
            }
            return View(_mapper.Map<OrderDto>(order));
        }

    }
}