using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace OnlineShoppingCart.Utils
{
    public static class HtmlHelper
    {
        public static string HtmlDocument(string title, string content)
        {
            return $@"
                    <!DOCTYPE html>
                    <html>
                        <head>
                            <meta charset=""UTF-8"">
                            <title>{title}</title>
                            <link rel=""shortcut icon"" type=""image/x-icon"" href=""~/assets/img/ic_a.png"">
                            <link rel=""stylesheet"" href=""/lib/bootstrap/dist/css/bootstrap.min.css"" />
                            <link rel=""stylesheet"" href=""/css/site.min.css"" />
                            <script src=""/lib/bootstrap/dist/js/bootstrap.bundle.min.js""></script>
                        </head>
                        <body>
                            {content}
                        </body>
                    </html>";
        }

        public static string HtmlMainContent(string header, string body)
        {
            return $@"
                        <div class=""container"">
                            <div class=""jumbotron"">
                                <h1 class=""display-4"">{header}</h1>
                                <hr class=""my-4"">
                                <p>{body}</p>
                            </div>
                        </div>
                        ";

        }

        // "content".HtmlTag() => <p>content</p>
        // "content".HtmlTag("div", "text-danger") => <div class="text-danger">content</div>
        public static string HtmlTag(this string content, string tag = "p", string? _class = null)
        {
            string? cls = (_class != null) ? $" class=\"{_class}\"" : null;
            return $"<{tag + cls}>{content}</{tag}>";
        }
        public static string Td(this string content, string? _class = null)
        {
            return content.HtmlTag("td", _class);
        }
        public static string Tr(this string content, string? _class = null)
        {
            return content.HtmlTag("tr", _class);
        }
        public static string Table(this string content, string? _class = null)
        {
            return content.HtmlTag("table", _class);
        }


    }
}