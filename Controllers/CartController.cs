using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Models;

namespace OnlineShoppingCart.Controllers
{
    [Route("/products")]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ApplicationDbContext _context;

        public CartController(ILogger<CartController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public const string CART_KEY = "cart";

        // get cartItem from session
        public List<CartItem>? GetCartItems()
        {
            var session = HttpContext.Session;
            string jsonCart = session.GetString(CART_KEY)!;
            if (jsonCart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsonCart);
            }
            return new List<CartItem>();
        }
        //delete cart in the session
        public void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CART_KEY);
        }

        //save Cart vao session
        public void SaveCartSession(List<CartItem> cartItems)
        {
            var session = HttpContext.Session;
            string jsonCart = JsonConvert.SerializeObject(cartItems);
            session.SetString(CART_KEY, jsonCart);
        }

        //them sp vao cart
        [Route("add-to-cart/{id}")]
        public async Task<IActionResult> AddToCart([FromRoute] string id)
        {
            var productExist = await _context.Products!.SingleOrDefaultAsync(p => p.Id == id);
            if (productExist == null)
            {
                return NotFound("Can not find product");
            }
            //add to cart
            var cart = GetCartItems();
            var cartItemExist = cart!.Find(p => p.Product.Id == id);
            if (cartItemExist != null)
            {
                //da ton tai, tang them 1;
                cartItemExist.Quantity++;
            }
            else
            {
                //add new
                cart.Add(new CartItem()
                {
                    Quantity = 1,
                    Product = productExist
                });
            }
            //luu vao session
            SaveCartSession(cart);

            return RedirectToAction(nameof(Cart));
        }

        //xoa cart
        [Route("/remove-cart/{id}", Name = "RemoveCart")]
        public IActionResult RemoveCart([FromRoute] string id)
        {
            //xu ly xoa cart
            var cart = GetCartItems();
            var cartItemExist = cart!.Find(p => p.Product.Id == id);
            if (cartItemExist != null)
            {
                cart.Remove(cartItemExist);
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        //cap nhat cart
        [Route("/update-cart", Name = "UpdateCart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] string id, [FromForm] int quantity)
        {
            //cap nhat so luong sp
            var cart = GetCartItems();
            var cartItemExist = cart!.Find(p => p.Product.Id == id);
            if (cartItemExist != null)
            {
                cartItemExist.Quantity = quantity;
            }
            SaveCartSession(cart);
            return Ok();
        }

        //Display Cart page
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(GetCartItems());
        }


        [Route("/check-out")]
        public IActionResult CheckOut()
        {
            //process check out
            return View();
        }
    }
}