using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopifyInstaller.Models;
using ShopifyInstaller.Servcices;
using System;
using System.Threading.Tasks;

namespace ShopifyInstaller.Controllers
{
    [Route("api/[controller]")]
    public class ShopifiesController : Controller
    {
        private readonly IShopifyService _shopifyService;
        private readonly ILogger<ShopifiesController> _logger;
        private readonly IHostingEnvironment _env;

        public ShopifiesController(IShopifyService shopifyService, ILogger<ShopifiesController> logger, IHostingEnvironment env)
        {
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

            await _shopifyService.InstallAsync(shop, code);

            var content = @"<iframe id=""chatigy-iframe"" scrolling=""no"" src=""https://chatigy.justinchasez.space/chatbox?tenantDomain=genk.vn""></iframe>";

            return new ContentResult()
            {
                Content = content,
                ContentType = "text/html",
            };
        }

        [HttpGet("uninstall")]
        public async Task<IActionResult> Uninstall()
        {
            _logger.LogInformation("uninstalling...");

            return Ok();
        }
    }
}
