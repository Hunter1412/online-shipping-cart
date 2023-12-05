using Newtonsoft.Json;
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
        // Key lưu chuỗi json của Cart
        public const string CART_KEY = "cart";

        // Lấy cart từ Session (danh sách CartItem)
        public List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CART_KEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
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
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CART_KEY, jsoncart);
        }
    }
}