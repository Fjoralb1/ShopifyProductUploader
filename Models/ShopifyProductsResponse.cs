using Newtonsoft.Json;

namespace ShopifyTool.Models
{
    public class ShopifyProductsResponse
    {
        [JsonProperty("products")]
        public List<ShopifyProduct> Products { get; set; }
    }

    public class ShopifyProduct
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("variants")]
        public List<ShopifyVariant> Variants { get; set; }
    }

    public class ShopifyVariant
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("inventory_quantity")]
        public int InventoryQuantity { get; set; }
    }

}
