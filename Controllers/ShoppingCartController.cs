using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Models.DTOs;
using OnlineShoppingCart.Utils;


namespace OnlineShoppingCart.Controllers
{
    [Route("/products")]
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;



        public ShoppingCartController(ILogger<ShoppingCartController> logger, ApplicationDbContext context, CartService cartService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> GetProductAsync(string id)
        {
            var productList = await _unitOfWork.Products.GetAll("Inventories");
            var productDtoList = productList == null ? new List<ProductDto>()
                : productList.Select(p => _mapper.Map<ProductDto>(p)).OrderByDescending(p => p.CreateAt).ToList();

            var productDto = productDtoList.Where(p => p.Id == id).FirstOrDefault();
            return productDto ?? new ProductDto();
        }

        public async Task<int> StockQuantity(string id)
        {
            var productExist = await GetProductAsync(id);
            int stock = productExist.Inventories.Select(x => x.Quantity).Sum() ?? 0;
            return stock;
        }

        /// Thêm sản phẩm vào cart
        [Route("/add-cart", Name = "addcart")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(string id, int quantity)
        {
            var product = await _context.Products!.Where(p => p.Id == id)!.FirstOrDefaultAsync();
            if (product == null)
                return NotFound("Can't found");

            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.Product!.Id == id);

            var qty = quantity > 0 ? quantity : 0;
            // var stock = await  StockQuantity(id);

            // if (stock < qty){}

            // _logger.LogInformation($"CHeck >>>>>>> {stock}");

            var image = await _context.Images!.Where(x => x.ProductId == id).AsNoTracking().FirstAsync();
            string imageName = image.ImageName!;

            if (cartItem != null)
            {
                // Đã tồn tại, tăng thêm qty
                cartItem.Quantity += qty;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { Quantity = qty, Product = product, ImageName = imageName });
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
                // Đã tồn tại, tăng thêm quantity
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
            var cartItem = cart.Find(p => p.Product!.Id == id);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }

            _cartService.SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }



        [Route("/checkout", Name = "checkout")]
        public IActionResult CheckOut()
        {
            var cart = _cartService.GetCartItems();
            if (cart.Count == 0)
            {
                return RedirectToAction("Shop", "Home");
            }
            // Xử lý khi đặt hàng
            return View();
        }


        //voucher
        [Route("/voucher", Name = "checkvoucher")]
        [HttpPost]
        public async Task<ActionResult> CheckVoucher([FromForm] string code)
        {
            _logger.LogInformation($"check voucher>> {code}");
            Voucher? voucher = await _context.Vouchers!.SingleOrDefaultAsync(x => x.Code == code);
            if (voucher == null)
            {
                return Json(new { error = "Voucher is invalid" });
            }
            if (voucher.ExpDate <= DateTime.Now)
            {
                return Json(new { error = "Voucher was exp-date" });
            }
            var cart = _cartService.GetCartItems();
            double total = 0.00;
            foreach (var item in cart)
            {
                total += item.Quantity * (double)(item.Product!.Price! - item.Product?.Promotion);
            }
            if (voucher.MinimumBill > total)
            {
                return Json(new
                {
                    error = $"The total must be over {voucher.MinimumBill}"
                });
            }
            _cartService.ClearVoucher();

            _cartService.SaveVoucherSession(voucher);
            return Ok(new
            {
                code = code,
                discount = voucher.Discount,
                value = (total - voucher.Discount)
            });
        }




        //payment
        public IActionResult Success(string paymentId, string token, string payerID)
        {
            ViewData["PaymentId"] = paymentId;
            ViewData["token"] = token;
            ViewData["PayerId"] = payerID;
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PayUsingCard(double amount = 10)
        {
            try
            {
                if (amount == 0)
                {
                    TempData["error"] = "Plz enter amount";
                    return RedirectToAction("Index");
                }

                string returnUrl = "http://localhost:5051/ShoppingCart/Success";
                string cancelUrl = "http://localhost:5051/ShoppingCart/Cancel";

                //create order
                var createdPayment = await _unitOfWork.PaypalServices.CreateOrderAsync(amount, returnUrl, cancelUrl);

                string approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.ToLower() == "approval_url")?.href;

                if (!string.IsNullOrEmpty(approvalUrl))
                {
                    return Redirect(approvalUrl);
                }
                else
                {
                    TempData["error"] = "Failure to initiate Paypal payment";
                }
            }
            catch (System.Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), "Home");
        }
    }

}