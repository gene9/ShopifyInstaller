using System.Collections.Generic;

namespace ShopifyInstaller.Settings
{
    public class ShopifyConfig
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string RedirectUri { get; set; }

        public string Scopes { get; set; }
        public string Event { get; set; }
        public string ScriptUrl { get; set; }
        public string SuccessUri { get; set; }
        public string ErrorUri { get; set; }
    }

    public static class ShopifyConfigExtenstions
    {
        public static IEnumerable<string> GetScopes(this ShopifyConfig shopifyConfig)
        {
            if (string.IsNullOrWhiteSpace(shopifyConfig.Scopes))
            {
                return new string[0];
            }

            return shopifyConfig.Scopes.Split(',');
        }
    }
}
