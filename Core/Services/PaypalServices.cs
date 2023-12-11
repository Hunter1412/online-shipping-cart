using OnlineShoppingCart.Utils;
using PayPal.Api;

namespace OnlineShoppingCart.Core.Services
{
    public class PaypalServices : IPaypalServices
    {
        protected readonly APIContext ApiContext;
        protected Payment Payment;
        protected readonly IConfiguration _configuration;

        public PaypalServices(IConfiguration configuration)
        {
            _configuration = configuration;

            var clientId = _configuration.GetValue<string>("PayPal:Key");
            var clientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");

            string accessToken = new OAuthTokenCredential(
                clientId,
                clientSecret,
                new Dictionary<string, string>()
                {
                    {"mode",mode}
                }
            ).GetAccessToken();

            ApiContext = new APIContext(accessToken)
            {
                Config = new Dictionary<string, string>(){
                    {"mode",mode}
                }
            };

        }

        public async Task<Payment> CreateOrderAsync(double amount, string returnUrl, string cancelUrl)
        {

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            itemList.items.Add(new Item()
            {
                name = "Membership Fee",
                currency = "USD",
                price = amount.ToString("0.00"),
                quantity = "1",
                sku = "membership"
            });

            // Adding description about the transaction
            Random random = new();
            var paypalOrderId = random.NextString(8);

            var transaction = new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(),
                amount = new Amount()
                {
                    currency = "USD",
                    total = amount.ToString("0.00"),
                    details = new Details()
                    {
                        subtotal = amount.ToString("0.00")
                    }
                },
                item_list = itemList
            };

            this.Payment = new Payment()
            {
                intent = "sale",
                payer = new Payer() { payment_method = "paypal" },
                redirect_urls = new RedirectUrls()
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                },
                transactions = new List<Transaction>() { transaction }
            };

            return this.Payment.Create(this.ApiContext);
        }

        public Payment ExecutePayment(string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            Payment = new Payment()
            {
                id = paymentId
            };
            return Payment.Execute(this.ApiContext, paymentExecution);
        }
    }
}