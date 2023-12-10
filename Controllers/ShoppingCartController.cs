using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly UserManager<AppUser> _userManager;



        public ShoppingCartController(ILogger<ShoppingCartController> logger, ApplicationDbContext context, CartService cartService, IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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
            return Json(new
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
        [Authorize]
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


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder([FromForm] FormCheckOut form)
        {
            _logger.LogInformation(">>>>>>>>>>CheckOut action>>>>>>>>>>>>>>>>>>>>");
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please enter your information.");
                return RedirectToAction(nameof(CheckOut));
            }

            string SubStringAddress(string input)
            {
                return input[(input.LastIndexOf("--") + 2)..];
            }

            AppUser? user = await _unitOfWork.Users.Get(x => x.Email == form.Email);

            var shipping = new Shipping();
            shipping.Id = Guid.NewGuid().ToString();
            shipping.Name = form.Name;
            shipping.Email = form.Email;
            shipping.Phone = form.Phone;

            shipping.City = SubStringAddress(form.City!);
            shipping.District = SubStringAddress(form.District!);
            shipping.Wards = SubStringAddress(form.Ward!);
            shipping.Address = form.Address;
            shipping.Note = form.Note;

            shipping.DeliveryType = form.DeliveryType;
            shipping.ShippingFee = 0; //khuyen mai

            await _unitOfWork.Shippings.Add(shipping);

            List<CartItem> carts = _cartService.GetCartItems();
            Voucher? voucher = _cartService.GetVoucher();
            var voucherDiscount = voucher != null ? voucher.Discount : 0;

            //save order
            var guidNumber = Convert.ToString((new Random()).Next(100000000));
            var order = new Order
            {
                Id = guidNumber,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending", //dang xu ly
                OrderTotal = _cartService.CalculateTotal(carts) - voucherDiscount,
                PaymentStatus = "Finish", //-Await payment | Finish | Refund,
                PaymentMethod = form.PaymentMethod,
                ShippingId = shipping.Id,
                VoucherId = voucher?.Id,
                UserId = user.Id,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };

            _cartService.ClearVoucher(); //delete voucher used
            await _unitOfWork.Orders.Add(order);
            await _unitOfWork.CompleteAsync();

            foreach (var cart in carts)
            {
                var discount = cart.Product?.Promotion > 0 ? cart.Product.Promotion : 0;
                //order detail
                var od = new OrderDetail
                {
                    // a unique 16 digit order number
                    //includes the type of the delivery (1 digit)
                    //that the user has opted for, Product id (7 digit), and the order number (8 digit)
                    OrderNumber = form.DeliveryType + cart.Product!.Id + order.Id,
                    OrderId = order.Id,
                    ProductId = cart.Product!.Id,
                    Quantity = cart.Quantity,
                    Price = (double)(cart.Product.Price! - discount)
                };
                await _unitOfWork.OrderDetails.Add(od);
                await _unitOfWork.CompleteAsync();

                //stock warehouse
                var inventory = new Inventory
                {
                    Id = Guid.NewGuid().ToString(),
                    DateAt = DateTime.Now,
                    ProductId = cart.Product.Id,
                    Quantity = -cart.Quantity,
                    Note = "Output"
                };
                await _unitOfWork.Inventory.Add(inventory);
                await _unitOfWork.CompleteAsync();
            }
            //Clear Session (carts, voucher)
            _cartService.ClearCart();
            _cartService.ClearVoucher();


            return RedirectToAction("Index", "Home");
        }

        




        //voucher
        [Route("/voucher", Name = "checkvoucher")]
        [HttpPost]
        public async Task<ActionResult> CheckVoucher([FromForm] string code)
        {
            _logger.LogInformation($"check voucher>> {code}");
            code = code.Trim().ToUpper();
            var voucher = await _unitOfWork.Vouchers.Get(x => x.Code!.ToUpper() == code);
            // Voucher? voucher = await _context.Vouchers!.SingleOrDefaultAsync(x => x.Code == code);
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
        [HttpGet]
        public IActionResult Success(string paymentId, string token, string PayerID)
        {
            ViewData["paymentId"] = paymentId;
            ViewData["token"] = token;
            ViewData["PayerId"] = PayerID;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PayUsingCard(double amount = 1)
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

            return RedirectToAction(nameof(CheckOut));
        }
    }

}