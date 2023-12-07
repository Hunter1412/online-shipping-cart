using PayPal.Api;

namespace OnlineShoppingCart.Core.Services
{
    public interface IPaypalServices
    {
        Task<Payment> CreateOrderAsync(double amount, string returnUrl, string cancelUrl);
    }
}