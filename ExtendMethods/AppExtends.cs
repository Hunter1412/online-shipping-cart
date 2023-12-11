using System.Net;
using OnlineShoppingCart.Utils;

namespace OnlineShoppingCart.ExtendMethods
{
    public static class AppExtends
    {
        public static void AddStatusCodePages(this IApplicationBuilder app)
        {
            //tao ra noi dung tu error 400-599
            app.UseStatusCodePages(appError =>
            {
                appError.Run(async context =>
                {
                    var res = context.Response;
                    var code = res.StatusCode;
                    var content = @$"
                            <section style='display: flex;align-items: center;justify-content: center; flex-direction:column;margin-top: 20vh;'>
                                <h1 style='color:red;'>
                                    Error: {code} - {(HttpStatusCode)code}
                                </h1>
                                <h3><a href='/' style='text-decoration:none;'>Back to Home page</a></h3>
                                <br/>
                            </section>";
                    var html = HtmlHelper.HtmlDocument($"{code}", content);
                    await res.WriteAsync(html);
                });
            });
        }
    }
}