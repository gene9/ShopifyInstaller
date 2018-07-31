using System;
using System.Text;

namespace ShopifyInstaller.Helpers
{
    public class Base64UrlHelper
    {
        //public static string Base64ForUrlEncode(string str)
        //{
        //    byte[] encbuff = Encoding.UTF8.GetBytes(str);
        //    return HttpServerUtility.UrlTokenEncode(encbuff);
        //}

        public static string Decode(string base64String)
        {
            var decodedBytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(decodedBytes, 0, decodedBytes.Length);
        }
    }
}
