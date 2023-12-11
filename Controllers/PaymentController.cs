using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Utils;
using PayPal.Api;

namespace OnlineShoppingCart.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        protected readonly IConfiguration _configuration;
        protected readonly IUnitOfWork _unitOfWork;

        private readonly CartService _cartService;

        public PaymentController(ILogger<PaymentController> logger, IHttpContextAccessor context, IConfiguration configuration, IUnitOfWork unitOfWork, CartService cartService)
        {
            _logger = logger;
            this.httpContextAccessor = context;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }

        public IActionResult PaymentWithPaypal(string? Cancel = null, string PayerID = "", string guid = "")
        {
            var clientId = _configuration.GetValue<string>("PayPal:Key");
            var clientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");

            //getting the apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext(clientId, clientSecret, mode);
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal
                //Payer Id will be returned when payment proceeds or click to pay
                string payerId = PayerID;
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class
                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Payment/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session
                    //which will be used in the payment execution
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;
                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid
                    httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment
                    var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
                    //If executed payment failed then we will show payment failure message to user
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("FailureView", "ShoppingCart");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error PaymentWithPayPal method");
                return RedirectToAction("FailureView", "ShoppingCart");
            }
            //on successful payment, show success page to user.
            return RedirectToAction("Success", "ShoppingCart");
        }


        private PayPal.Api.Payment Payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            Payment = new Payment()
            {
                id = paymentId
            };
            return Payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var carts = _cartService.GetCartItems();
            var voucher = _cartService.GetVoucher();
            double discount = voucher != null ? voucher.Discount : 0.00;
            double total = _cartService.CalculateTotal(carts) - discount;
            //create itemlist and add item objects to it
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc
            foreach (var cartItem in carts)
            {
                itemList.items.Add(new Item()
                {
                    name = cartItem.Product!.Name,
                    currency = "USD",
                    price = (cartItem.Product.Price - cartItem.Product.Promotion).ToString(),
                    quantity = cartItem.Quantity.ToString(),
                    sku = cartItem.Product.Id
                });
            }
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = total.ToString()
            };
            //Final amount with details
            var amount = new Amount()
            {
                currency = "USD",
                total = total.ToString(), // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction
            var paypalOrderId = DateTime.Now.Ticks;

            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No
                amount = amount,
                item_list = itemList
            });
            Payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext
            return Payment.Create(apiContext);
        }

    }
}