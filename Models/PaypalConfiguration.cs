using PayPal.Api;

namespace OnlineShoppingCart.Models
{
    public static class PaypalConfiguration
    {
        static PaypalConfiguration() { }
        public static Dictionary<string, string> GetConfig(string mode)
        {
            return new Dictionary<string, string>(){
                {"mode",mode}
            };
        }

        private static string GetAccessToken(string clientId, string clientSecret, string mode)
        {
            string accessToken = new OAuthTokenCredential(
                clientId,
                clientSecret,
                new Dictionary<string, string>()
                {
                    {"mode",mode}
                }
            ).GetAccessToken();
            return accessToken;
        }

        public static APIContext GetAPIContext(string clientId, string clientSecret, string mode)
        {
            APIContext apiContext = new(GetAccessToken(clientId, clientSecret, mode))
            {
                Config = GetConfig(mode)
            };
            return apiContext;
        }
    }
}