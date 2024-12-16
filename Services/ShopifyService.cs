using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShopifyTool.Helpers;
using ShopifyTool.Models;
using System.Net;
using System.Text;

namespace ShopifyTool.Services
{
    public class ShopifyService
    {
        private readonly string _apiVersion;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ShopifyService> _logger;

        public ShopifyService(HttpClient httpClient, IOptions<ShopifySettings> settings, ILogger<ShopifyService> logger)
        {
            var shopifySettings = settings.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(shopifySettings.ShopUrl);
            _httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", shopifySettings.AccessToken);
            _apiVersion = shopifySettings.APIVersion;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> CreateProductAsync(ProductDTO product)
        {
            try
            {
                //Load existing SKUs from shoify admin API.
                var existingSkus = await GetExistingSkusAsync();

                //Validate the product
                var validationErrors = ProductHelper.ValidateProduct(product, existingSkus);

                //If there are errors in validation stop the process.
                if (validationErrors.Count > 0)
                {
                    //Log the validation errors.
                    var errorMessage = string.Join("; ", validationErrors);
                    _logger.LogWarning($"Product validation failed: {errorMessage}");

                    var errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            Message = "Validation errors occurred.",
                            Errors = validationErrors
                        }), Encoding.UTF8, "application/json")
                    };
                    return errorResponse;
                }

                var payload = new
                {
                    product = product
                };

                //If there are no validation errors continue with the product upload.
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BuildRequestUri("products.json"), content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to create product: {errorContent}");

                    //Return detailed response.
                    var errorResponse = new HttpResponseMessage(response.StatusCode)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            Message = "Failed to create the product in your shopify store.",
                            ErrorDetails = errorContent
                        }), Encoding.UTF8, "application/json")
                    };

                    return errorResponse;
                }

                _logger.LogInformation("Product created successfully.");

                return response;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the product.");

                // Return a 500 Internal Server Error.
                var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        Message = "An unexpected error occurred.",
                        Exception = ex.Message
                    }), Encoding.UTF8, "application/json")
                };
                return errorResponse;
            }
        }

        /// <summary>
        /// Retrieves the full list of existing SKUs from the store using api callls to the shopify APIs.
        /// </summary>
        /// <returns>List of string consisting of Product SKUs.</returns>
        /// <exception cref="Exception">Throw exception if the response from shopify admin API is not sucessful.</exception>
        public async Task<List<string>> GetExistingSkusAsync()
        {
            _logger.LogInformation("Fetching existing SKUs from Shopify.");

            try
            {
                var skus = new List<string>();
                string? endpoint = BuildRequestUri("products.json").ToString();
                //Loop for each page to fetch all products (shopify limit is 200).
                while (!string.IsNullOrEmpty(endpoint))
                {
                    var response = await _httpClient.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = $"Failed to fetch products. Status: {response.StatusCode}, Error: {await response.Content.ReadAsStringAsync()}";
                        _logger.LogError(error);
                        throw new Exception(error);
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var productsResponse = JsonConvert.DeserializeObject<ShopifyProductsResponse>(content);

                    // Extract SKUs from variants
                    foreach (var product in productsResponse.Products)
                    {
                        skus.AddRange(product.Variants.Select(v => v.Sku));
                    }

                    // Check for pagination (Link header)
                    if (response.Headers.TryGetValues("Link", out var links))
                        endpoint = ProductHelper.GetNextPageUrl(links.FirstOrDefault());
                    else
                        endpoint = null; // No more pages
                }

                _logger.LogInformation($"Fetched {skus.Count} SKUs successfully.");
                return skus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching SKUs.");
                throw;
            }
        }

        /// <summary>
        /// Combines the base address with the provided endpoint to construct the full URI.
        /// </summary>
        /// <param name="endpoint">The specific Shopify Admin API endpoint to access.</param>
        /// <returns>Returns the fully constructed URI.</returns>
        private Uri BuildRequestUri(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be null or empty.", nameof(endpoint));

            return new Uri($"{_httpClient.BaseAddress}admin/api/{_apiVersion}/{endpoint}");
        }
    }
}
