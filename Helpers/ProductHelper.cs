using Newtonsoft.Json;
using ShopifyTool.Models;
using System.Net.Http;

namespace ShopifyTool.Helpers
{
    public static class ProductHelper
    {
        /// <summary>
        /// Validates the product for required fields and constraints.
        /// </summary>
        /// <param name="product">The product to validate.</param>
        /// <param name="existingSkus">A list of existing SKUs to check for duplicates.</param>
        /// <returns>A list of validation error messages.</returns>
        public static List<string> ValidateProduct(ProductDTO product, List<string> existingSkus)
        {
            var errors = new List<string>();

            // Validate product title
            if (string.IsNullOrWhiteSpace(product.Title))
                errors.Add("Product title is required.");

            // Validate product variants
            if (product.Variants == null || product.Variants.Count == 0)
            {
                errors.Add("At least one variant is required.");
            }
            else
            {
                foreach (var variant in product.Variants)
                {
                    // Validate SKU
                    if (string.IsNullOrWhiteSpace(variant.SKU))
                        errors.Add("SKU is required for all variants.");
                    else if (existingSkus.Contains(variant.SKU))
                        errors.Add($"Duplicate SKU detected: {variant.SKU}");

                    // Validate price
                    if (variant.Price <= 0)
                        errors.Add("Variant price must be greater than 0.");

                    // Validate inventory
                    if (variant.InventoryQuantity < 0)
                        errors.Add("Inventory quantity cannot be negative.");
                }
            }

            // Validate images
            if (product.Images != null && product.Images.Count != 0)
            {
                foreach (var image in product.Images)
                {
                    if (string.IsNullOrWhiteSpace(image.Src) && string.IsNullOrWhiteSpace(image.Attachment))
                        errors.Add("Each image must have either a URL (Src) or a Base64 attachment.");
                }
            }

            return errors;
        }

        public static string GetNextPageUrl(string linkHeader)
        {
            if (string.IsNullOrEmpty(linkHeader)) return null;

            var links = linkHeader.Split(',');
            foreach (var link in links)
            {
                var parts = link.Split(';');
                if (parts.Length == 2 && parts[1].Trim().Equals("rel=\"next\"", StringComparison.OrdinalIgnoreCase))
                {
                    return parts[0].Trim('<', '>', ' ');
                }
            }

            return null; // No "next" link found
        }
    }
}
