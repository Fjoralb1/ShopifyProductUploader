namespace ShopifyTool.Services
{
    public class TokenService
    {
        private string _accessToken;

        public string AccessToken
        {
            get => _accessToken;
            set => _accessToken = value;
        }
    }
}
