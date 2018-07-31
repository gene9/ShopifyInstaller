using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopifyInstaller.Servcices;
using ShopifyInstaller.Settings;
using System;
using System.Threading.Tasks;

namespace ShopifyInstaller.Controllers
{
    [Route("api/[controller]")]
    public class ShopifiesController : Controller
    {
        private readonly IShopifyService _shopifyService;
        private readonly ILogger<ShopifiesController> _logger;
        private readonly ShopifyConfig _shopifyConfig;
        private readonly IHostingEnvironment _env;

        public ShopifiesController(IOptions<ShopifyConfig> shopifyConfigAccessor, IShopifyService shopifyService, ILogger<ShopifiesController> logger, IHostingEnvironment env)
        {
            _shopifyConfig = shopifyConfigAccessor?.Value ?? throw new ArgumentNullException(nameof(shopifyConfigAccessor));
            _shopifyService = shopifyService ?? throw new ArgumentNullException(nameof(shopifyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env;
        }

        [HttpGet("install")]
        public IActionResult Install(string shop)
        {
            var url = _shopifyService.BuildAuthorizationUrl(shop);
            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string shop, string code)
        {
            _logger.LogInformation($"shop: {shop}");

            _logger.LogInformation($"code: {code}");

            var result = await _shopifyService.InstallAsync(shop, code);

            if (!result)
            {
                return Redirect(_shopifyConfig.ErrorUri);
            }

            return Redirect(_shopifyConfig.SuccessUri);
        }
    }
}
