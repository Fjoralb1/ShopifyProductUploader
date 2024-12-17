using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShopifyTool.Models;
using ShopifyTool.Services;
using System.Text;

namespace ShopifyTool.Controllers
{
    public class OAuthController : Controller
    {
        private readonly ShopifySettings _shopifySettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenService _tokenService;

        public OAuthController(IOptions<ShopifySettings> options, IHttpClientFactory httpClientFactory, TokenService tokenService)
        {
            _shopifySettings = options.Value;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }

        // Redirect user to Shopify for authorization
        public IActionResult Authorize()
        {
            var authorizationUrl = $"{_shopifySettings.ShopUrl}/admin/oauth/authorize?" +
                                   $"client_id={_shopifySettings.ClientID}" +
                                   $"&scope={_shopifySettings.Scopes}" +
                                   $"&redirect_uri={Uri.EscapeDataString(_shopifySettings.RedirectUri)}";

            return Redirect(authorizationUrl);
        }

        // Handle Shopify's callback and exchange code for an access token
        [HttpGet]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code is missing.");
            }

            var tokenUrl = $"{_shopifySettings.ShopUrl}/admin/oauth/access_token";
            var client = _httpClientFactory.CreateClient();

            var tokenRequestBody = new
            {
                client_id = _shopifySettings.ClientID,
                client_secret = _shopifySettings.ClientSecret,
                code
            };

            var content = new StringContent(JsonConvert.SerializeObject(tokenRequestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(tokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                
                return StatusCode((int)response.StatusCode, $"Error fetching access token: {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(responseBody);

            //Update the access token in appsettings.
            if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                _tokenService.AccessToken = tokenResponse.AccessToken;
                return RedirectToAction("Index", "Shopify");
            }

            return RedirectToAction("Error");
        }

        private class AccessTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
        }
    }
}
