using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineShoppingCart.Models
{
    public class ViewPaymentStatusModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public SelectList OrderStatusList { get; set; } = null!;
    }
}