using Newtonsoft.Json;

namespace ShopifyTool.Models
{
    public class ProductDTO
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body_html")]
        public string Description { get; set; }
        [JsonProperty("variants")]
        public List<ProductVariant> Variants { get; set; }
        [JsonProperty("images")]
        public List<ProductImage> Images { get; set; }

        public ProductDTO()
        {
            Variants = [];
            Images = [];
        }
    }
}
