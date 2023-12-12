using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using OrderEntity = OnlineShoppingCart.Data.Entities.Order;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Models.DTOs;
using OnlineShoppingCart.Utils;
using PayPal.Api;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;


namespace OnlineShoppingCart.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly UserManager<AppUser> _userManager;

        protected readonly IConfiguration _configuration;

        public ShoppingCartController(ILogger<ShoppingCartController> logger, ApplicationDbContext context, CartService cartService, IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _emailSender = emailSender;
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
                return RedirectToAction("Index", "Shop", new { categoryslug = "" });
            }
            // Xử lý khi đặt hàng
            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder([FromForm] FormCheckOut form)
        {
            _logger.LogInformation(">>>>>>>>>>CheckOut action>>>>>>>>>>>>>>>>>>>>");
            TempData["start"] = true;
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please enter your information.");
                return RedirectToAction(nameof(CheckOut));
            }

            static string SubStringAddress(string input)
            {
                return input[(input.LastIndexOf("--") + 2)..];
            }

            AppUser? user = await _unitOfWork.Users.Get(x => x.Email == form.Email);

            var shipping = new Shipping
            {
                Id = Guid.NewGuid().ToString(),
                Name = form.Name,
                Email = form.Email,
                Phone = form.Phone,

                City = SubStringAddress(form.City!),
                District = SubStringAddress(form.District!),
                Wards = SubStringAddress(form.Ward!),
                Address = form.Address,
                Note = form.Note,

                DeliveryType = form.DeliveryType,
                ShippingFee = 0 //khuyen mai
            };

            await _unitOfWork.Shippings.Add(shipping);

            List<CartItem> carts = _cartService.GetCartItems();
            Voucher? voucher = _cartService.GetVoucher();
            var voucherDiscount = voucher != null ? voucher.Discount : 0;

            var paymentId = _httpContextAccessor.HttpContext.Session.GetString("payment");
            var orderId = _httpContextAccessor.HttpContext.Session.GetString("orderid");

            var paymentStatus = paymentId != null ? "Finish" : "Await Payment";

            //save order
            Random random = new();
            var guidNumber = orderId ?? random.NextString(8);

            var order = new OrderEntity
            {
                Id = guidNumber,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending", //dang xu ly
                OrderTotal = _cartService.CalculateTotal(carts) - voucherDiscount,
                PaymentStatus = paymentStatus, //-Await payment | Finish | Refund,
                PaymentMethod = form.PaymentMethod,
                ShippingId = shipping.Id,
                VoucherId = voucher?.Id,
                UserId = user.Id,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
            };

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

            //send mail
            var title = $"ORDER CONFIRMATION - {order.Id}";
            var body = @$"
                <h3>Dear {form.Name},</h3>
                <p>Our shop sends you this letter to confirm your order date {String.Format("{0:MM/dd/yyyy}", DateTime.Now)}.</p>
                <p>If we do not receive your notice of change or cancellation
                of your order within 7 days of the date you receive this letter,
                we will proceed with delivery of the goods you have ordered on the date stated.</p>
                <p>Sincerely thank you,</p>

                <b>Online Shopping Cart</b>
            ";
            await _emailSender.SendEmailAsync(form.Email!, title, body);

            //Clear Session (carts, voucher)
            _cartService.ClearSessionShoppingCart();

            TempData["start"] = false;

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



        //payment for order1
        [Authorize]
        public IActionResult Success(string paymentId, string token, string PayerId)
        {
            string content = @$"
                <p><strong>Your transaction is successful.</strong></p>
                <hr />
                <p>Payment ID: {paymentId}</p>
                <p>Token ID:{token}</p>
                <p>Payer ID: {PayerId}</p>
            ";
            return ViewComponent("MessagePage", new OnlineShoppingCart.Components.MessagePage.Message
            {
                Title = "Thanks for making payment",
                HtmlContent = content,
                SecondWait = 5,
                UrlRedirect = "/checkout"
            });
        }

        [Authorize]
        public async Task<IActionResult> PayUsingCard()
        {
            _cartService.ClearPaymentId();
            try
            {
                //get value from session
                var carts = _cartService.GetCartItems();
                var voucher = _cartService.GetVoucher();
                double discount = voucher != null ? voucher.Discount : 0.00;
                double total = _cartService.CalculateTotal(carts) - discount;


                var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string returnUrl = $"{hostname}/ShoppingCart/Success";
                string cancelUrl = $"{hostname}/ShoppingCart/Cancel";

                //create order-paypal
                var createdPayment = await _unitOfWork.PaypalServices.CreateOrderAsync(total, returnUrl, cancelUrl);

                string approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.ToLower().Trim().Equals("approval_url"))?.href;

                if (!string.IsNullOrEmpty(approvalUrl))
                {
                    _httpContextAccessor.HttpContext!.Session.SetString("payment", createdPayment.id);
                    return Redirect(approvalUrl);
                }
                else
                {
                    TempData["error"] = "Failure to initiate Paypal payment";
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error PayUsingCard method");
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        // //payment for order
        // public IActionResult FailureView()
        // {
        //     return View();
        // }

        // public ActionResult PaymentWithPaypal(string? Cancel = null, string PayerID = "", string guid = "")
        // {
        //     var clientId = _configuration.GetValue<string>("PayPal:Key");
        //     var clientSecret = _configuration.GetValue<string>("PayPal:Secret");
        //     var mode = _configuration.GetValue<string>("PayPal:mode");

        //     //getting the apiContext
        //     APIContext apiContext = PaypalConfiguration.GetAPIContext(clientId, clientSecret, mode);
        //     try
        //     {
        //         string payerId = PayerID;
        //         if (string.IsNullOrEmpty(payerId))
        //         {
        //             string baseURI = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + "/ShoppingCart/PaymentWithPayPal?";
        //             var guidd = Convert.ToString((new Random()).Next(100000));
        //             guid = guidd;

        //             var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
        //             var links = createdPayment.links.GetEnumerator();

        //             string paypalRedirectUrl = null;
        //             while (links.MoveNext())
        //             {
        //                 Links lnk = links.Current;
        //                 if (lnk.rel.ToLower().Trim().Equals("approval_url"))
        //                 {
        //                     paypalRedirectUrl = lnk.href;
        //                 }
        //             }
        //             _httpContextAccessor.HttpContext!.Session.SetString("payment", createdPayment.id);
        //             return Redirect(paypalRedirectUrl);
        //         }
        //         else
        //         {
        //             var paymentId = _httpContextAccessor.HttpContext!.Session.GetString("payment");
        //             var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
        //             if (executedPayment.state.ToLower() != "approved")
        //             {
        //                 _logger.LogError("Error PaymentWithPayPal method, do not approval");
        //                 return View("PaymentFailed");
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error PaymentWithPayPal method");
        //         return View("FailureView");
        //     }

        //     //on successful payment, show success page to user.
        //     return RedirectToAction("Success", "ShoppingCart");
        // }

        // private PayPal.Api.Payment Payment;
        // private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        // {
        //     var paymentExecution = new PaymentExecution()
        //     {
        //         payer_id = payerId
        //     };
        //     Payment = new Payment()
        //     {
        //         id = paymentId
        //     };
        //     return Payment.Execute(apiContext, paymentExecution);
        // }

        // private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        // {
        //     var carts = _cartService.GetCartItems();
        //     var voucher = _cartService.GetVoucher();
        //     double discount = voucher != null ? voucher.Discount : 0.00;
        //     double total = _cartService.CalculateTotal(carts) - discount;
        //     //create itemlist and add item objects to it
        //     var itemList = new ItemList()
        //     {
        //         items = new List<Item>()
        //     };
        //     //Adding Item Details like name, currency, price etc
        //     foreach (var cartItem in carts)
        //     {
        //         itemList.items.Add(new Item()
        //         {
        //             name = cartItem.Product!.Name,
        //             currency = "USD",
        //             price = (cartItem.Product.Price - cartItem.Product.Promotion).ToString(),
        //             quantity = cartItem.Quantity.ToString(),
        //             sku = cartItem.Product.Id
        //         });
        //     }
        //     var payer = new Payer()
        //     {
        //         payment_method = "paypal"
        //     };
        //     // Configure Redirect Urls here with RedirectUrls object
        //     var redirUrls = new RedirectUrls()
        //     {
        //         cancel_url = redirectUrl + "&Cancel=true",
        //         return_url = redirectUrl
        //     };
        //     // Adding Tax, shipping and Subtotal details
        //     var details = new Details()
        //     {
        //         tax = "0",
        //         shipping = "0",
        //         subtotal = total.ToString()
        //     };
        //     //Final amount with details
        //     var amount = new Amount()
        //     {
        //         currency = "USD",
        //         total = total.ToString(), // Total must be equal to sum of tax, shipping and subtotal.
        //         details = details
        //     };
        //     var transactionList = new List<Transaction>();
        //     // Adding description about the transaction
        //     Random random = new();
        //     var paypalOrderId = random.NextString(8);
        //     // saving the payID in the key
        //     _httpContextAccessor.HttpContext.Session.SetString("orderid", paypalOrderId);

        //     transactionList.Add(new Transaction()
        //     {
        //         description = $"Invoice #{paypalOrderId}",
        //         invoice_number = paypalOrderId.ToString(), //Generate an Invoice No
        //         amount = amount,
        //         item_list = itemList
        //     });
        //     Payment = new Payment()
        //     {
        //         intent = "sale",
        //         payer = payer,
        //         transactions = transactionList,
        //         redirect_urls = redirUrls
        //     };
        //     // Create a payment using a APIContext
        //     return Payment.Create(apiContext);
        // }

    }

}