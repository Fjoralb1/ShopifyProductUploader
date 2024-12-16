using Newtonsoft.Json;

namespace ShopifyTool.Models
{
    public class ProductVariant
    {
        [JsonProperty("sku")]
        public string SKU { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("inventory_quantity")]
        public int InventoryQuantity { get; set; }
    }
}
