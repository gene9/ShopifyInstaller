using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopifyInstaller.Models;
using ShopifyInstaller.Settings;
using ShopifySharp;
using System;
using System.Threading.Tasks;

namespace ShopifyInstaller.Servcices
{
    public class ShopifyService : IShopifyService
    {
        private readonly ShopifyConfig _shopifyConfig;
        private readonly ILogger _logger;

        public ShopifyService(IOptions<ShopifyConfig> shopifyConfigAccessor, ILogger<ShopifyService> logger)
        {
            _shopifyConfig = shopifyConfigAccessor?.Value ?? throw new ArgumentNullException(nameof(shopifyConfigAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> InstallAsync(string shopUrl, string accessCode)
        {
            ShopService shopService = null;
            string accessToken = null;

            try
            {
                accessToken = await AuthorizationService.Authorize(accessCode, shopUrl, _shopifyConfig.ApiKey, _shopifyConfig.SecretKey);

                _logger.LogInformation($"accessToken: {accessToken}");

                if (string.IsNullOrEmpty(accessToken))
                {
                    return false;
                }

                shopService = new ShopService(shopUrl, accessToken);
                Shop shop = await shopService.GetAsync();

                string scriptUrl = _shopifyConfig.ScriptUrl;
                var scriptService = new ScriptTagService(shopUrl, accessToken);
                var existingScripts = await scriptService.ListAsync();
                foreach (var s in existingScripts)
                {
                    if (s.Id.HasValue)
                    {
                        await scriptService.DeleteAsync(s.Id.Value);
                    }
                }

                ScriptTag newScriptTag = await scriptService.CreateAsync(new ScriptTag
                {
                    Src = scriptUrl,
                    Event = _shopifyConfig.Event
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");

                await TryUninstallAsync(shopService, accessToken);

                return false;
            }
        }

        private async Task<bool> TryUninstallAsync(ShopService service, string accessToken)
        {
            try
            {
                if (service != null && !string.IsNullOrWhiteSpace(accessToken))
                {
                    await service.UninstallAppAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return false;
            }
        }

        public async Task<bool> UninstallAsync(string shopUrl, string accessCode)
        {
            try
            {
                string accessToken = await AuthorizationService.Authorize(accessCode, shopUrl, _shopifyConfig.ApiKey, _shopifyConfig.SecretKey);
                var service = new ShopService(shopUrl, accessToken);

                await service.UninstallAppAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return false;
            }
        }

        public string BuildAuthorizationUrl(string shopUrl)
        {
            var scopes = _shopifyConfig.GetScopes();

            var url = AuthorizationService.BuildAuthorizationUrl(scopes, shopUrl, _shopifyConfig.ApiKey, _shopifyConfig.RedirectUri).ToString();

            return url;
        }
    }
}
