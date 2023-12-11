using Newtonsoft.Json;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models;
using System.Text;

namespace OnlineShoppingCart.Utils
{
    public class CartService
    {
        private readonly IHttpContextAccessor _context;
        private readonly HttpContext HttpContext;


        public CartService(IHttpContextAccessor context)
        {
            _context = context;
            HttpContext = context.HttpContext;
        }
        public const string CART_KEY = "cart";
        public const string VOUCHER_KEY = "voucher";

        // Lấy cart từ Session (danh sách CartItem)
        public List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsonCart = session.GetString(CART_KEY);
            if (jsonCart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsonCart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
        public void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CART_KEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        public void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsonCart = JsonConvert.SerializeObject(ls);
            session.SetString(CART_KEY, jsonCart);
        }

        public double CalculateTotal(List<CartItem> carts)
        {
            if (carts == null || carts.Count == 0) return 0;
            double total = 0.00;
            foreach (var cart in carts)
            {
                var discount = cart?.Product?.Promotion > 0 ? cart.Product.Promotion : 0.00;
                total += cart!.Quantity * (double)(cart.Product!.Price - discount);
            }
            return total;
        }


        //lay voucher
        public Voucher? GetVoucher()
        {
            var session = HttpContext.Session;
            string? jsonVoucher = session.GetString(VOUCHER_KEY);
            if (jsonVoucher != null)
            {
                return JsonConvert.DeserializeObject<Voucher>(jsonVoucher);
            }
            return null;
        }
        //delete voucher
        public void ClearVoucher()
        {
            var session = HttpContext.Session;
            session.Remove(VOUCHER_KEY);
        }
        //save voucher
        public void SaveVoucherSession(Voucher voucher)
        {
            var session = HttpContext.Session;
            string jsonVoucher = JsonConvert.SerializeObject(voucher);
            session.SetString(VOUCHER_KEY, jsonVoucher);
        }

        public void ClearPaymentId()
        {
            var session = HttpContext.Session;
            session.Remove("payment");
        }

        //clear session all
        public void ClearSessionShoppingCart()
        {
            var session = HttpContext.Session;
            session.Remove(CART_KEY);
            session.Remove(VOUCHER_KEY);
            session.Remove("orderid");
            session.Remove("payment");
        }
    }
}