using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Utils
{
    public static class RandomExtension
    {
        public static string NextString(this Random random, int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}