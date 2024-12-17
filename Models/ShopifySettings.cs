namespace ShopifyTool.Models
{
    public class ShopifySettings
    {
        public string ShopUrl { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string APIVersion { get; set; }
        public string Scopes { get; set; }
        public string RedirectUri { get; set; }
    }
}
