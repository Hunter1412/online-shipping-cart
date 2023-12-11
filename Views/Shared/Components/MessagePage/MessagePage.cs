using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingCart.Components
{
    [ViewComponent]
    public class MessagePage : ViewComponent
    {
        public const string COMPONENTNAME = "MessagePage";
        // Dữ liệu nội dung trang thông báo
        public class Message
        {
            public string Title { set; get; } = "";     // Tiêu đề của Box hiện thị
            public string HtmlContent { set; get; } = "";         // Nội dung HTML hiện thị
            public string UrlRedirect { set; get; } = "/";        // Url chuyển hướng đến
            public int SecondWait { set; get; } = 3;              // Sau secondwait giây thì chuyển
        }
        public MessagePage() { }
        public IViewComponentResult Invoke(Message message)
        {
            this.HttpContext.Response.Headers.Add("REFRESH", $"{message.SecondWait};URL={message.UrlRedirect}");
            return View(message);
        }
    }
}