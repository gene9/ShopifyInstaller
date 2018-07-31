using System.Threading.Tasks;

namespace ShopifyInstaller.Servcices
{
    public interface IShopifyService
    {
        Task<bool> InstallAsync(string shopUrl, string accessCode);

        string BuildAuthorizationUrl(string shopUrl);
    }
}
