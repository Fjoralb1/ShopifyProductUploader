using Newtonsoft.Json;

namespace ShopifyTool.Models
{
    public class ProductImage
    {
        [JsonProperty("attachment")]
        public string? Attachment { get; set; }
        [JsonProperty("src")]
        public string? Src { get; set; }
    }
}
