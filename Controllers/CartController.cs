using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Utils;


namespace OnlineShoppingCart.Controllers
{
    [Route("/products")]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public CartController(ILogger<CartController> logger, ApplicationDbContext context, CartService cartService)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
        }


        /// Thêm sản phẩm vào cart
        [Route("/add-cart", Name = "addcart")]
        [HttpPost]
        public async Task<ActionResult> AddToCart(string id, int quantity)
        {
            var product = await _context.Products!.Where(p => p.Id == id)!.FirstOrDefaultAsync();

            if (product == null)
                return NotFound("Can't found");

            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.Product!.Id == id);

            var qty = quantity > 0 ? quantity : 0;


            if (cartItem != null)
            {
                // Đã tồn tại, tăng thêm qty
                cartItem.Quantity += qty;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { Quantity = qty, Product = product });
            }
            // Lưu cart vào Session
            _cartService.SaveCartSession(cart);
            // return RedirectToAction(nameof(Cart));
            return Ok(new
            {
                count = cart.Count,
                data = cart
            });
        }

        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(_cartService.GetCartItems());
        }

        /// Cập nhật
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] string id, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.Product!.Id == id);
            if (cartItem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartItem.Quantity = quantity;
            }
            _cartService.SaveCartSession(cart);
            return Ok(new { count = cart.Count });
        }

        /// xóa item trong cart
        [Route("/removecart/{id}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] string id)
        {
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.Product.Id == id);
            if (cartItem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartItem);
            }

            _cartService.SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }


        [Route("/checkout")]
        public IActionResult CheckOut()
        {
            // Xử lý khi đặt hàng
            return View();
        }


    }

}