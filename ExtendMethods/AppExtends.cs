using System.Net;

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
                    var content = @$"<html>
                        <head>
                            <title title>{code}</title>
                        </head>
                        <body>
                            <section style='display: flex;
                            align-items: center;
                            justify-content: center;'>
                                <h1 style='color:red;'>
                                    Error: {code} - {(HttpStatusCode)code}
                                </h1>
                                <br/>
                            </section>
                        </body>
                    </html>";
                    await res.WriteAsync(content);
                });
            });
        }
    }
}