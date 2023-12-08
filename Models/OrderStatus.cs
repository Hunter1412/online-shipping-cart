namespace OnlineShoppingCart.Models
{
    public enum OrderStatus
    {
        Pending,
        AwaitingPayment,
        OnHold,
        Shipped,
        Completed,
        Cancelled,
        Expired,
        Refunded
    }
}