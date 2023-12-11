using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayPal.Api;

namespace OnlineShoppingCart.Core.Services
{
    public interface IPaypalServices
    {
        Task<Payment> CreateOrderAsync(double amount, string returnUrl, string cancelUrl);

        Payment ExecutePayment(string payerId, string paymentId);
    }
}