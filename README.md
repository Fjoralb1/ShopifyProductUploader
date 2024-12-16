# Shopify Product Management Application

## Introduction

### Overview
This .NET Core application integrates with Shopify using the Admin REST API. It enables users to manage products in their Shopify store, including creating new products with variants and images.

### Features
- Product creation with variants and images.
- File upload or URL-based image management.
- Error handling for API integration.
- Validation of SKUs to avoid duplicates.

---

## Prerequisites

- A Shopify store with admin access.
- A registered private or custom app in the Shopify Partner Dashboard.
- API credentials: `Client ID`, `Client Secret`, and access to the store URL.
- Tools for testing:
  - `.NET Core SDK`
  - Visual Studio or any IDE supporting .NET Core
  - Ngrok (for local testing with HTTPS)

---

## Setup Instructions

### Shopify Configuration

1. Log in to your Shopify Partner Dashboard.
2. Register a new custom app or edit an existing one.
3. Add required API scopes (`write_products`, `read_products`).
4. Set the **Allowed Redirect URIs** to your application's callback URL.
5. Generate the API Key and Secret.

### Application Configuration

Update the `appsettings.json` file with the following fields:

```json
{
  "Shopify": {
    "ShopUrl": "https://your-shop-url.myshopify.com",
    "ApiKey": "your-api-key",
    "ApiPassword": "your-api-password",
    "APIVersion": "2024-10",
    "AccessToken: "AccessToken" --Still trying to manage on how to save it.
  }
}
```

---

## Deployment

### Local Development

1. Use Ngrok to expose your localhost:

```bash
ngrok http 5000
```

2. Update the Shopify app's redirect URI with the Ngrok forwarding URL (e.g., `https://your-ngrok-url.ngrok.io/oauth/callback`).

### Production

- Host on Azure, AWS, or any cloud platform.
- Use HTTPS for secure communication.
- Update Shopify settings with your production callback URL.

---

## Application Features

### Product Creation

- **Title**: *Mandatory field for the product name.*
- **Description**: *Mandatory field for product details.*
- **SKU and Price**: *Mandatory fields for the primary variant.*
- **Image**: *Supports either URL-based or file-based uploads (converted to Base64).*

### Validation

- Ensures SKU uniqueness.
- Verifies mandatory fields.
- Displays meaningful error messages for failed API calls.

### Error Handling

- Displays Shopify API errors in the UI.
- Logs errors using Serilog.

---

## Application Walkthrough

### Accessing the Application

- Navigate to the home page.
- Select "Create Product."

### Creating a Product

1. Fill in the product title, description, SKU, and price.
2. Add an image via URL or upload a file.
3. Click "Create Product" to submit.

### Success/Error Responses

- **On success**: Redirects to a confirmation page or displays a success message.
- **On error**: Displays the error message with details.

---

## API Workflow

### OAuth Authentication

1. Redirects the user to Shopify’s authorization page.
2. Retrieves and stores the access token securely.

### Product API Call

- **Endpoint**: `POST /admin/api/2024-10/products.json`
- **Headers**: Include `X-Shopify-Access-Token: <Access-Token>`
- **Body**:

```json
{
  "product": {
    "title": "Sample Product",
    "body_html": "Description",
    "variants": [
      {
        "sku": "unique-sku",
        "price": "19.99",
        "inventory_quantity": "3"
      }
    ],
    "images": [
      {
        "attachment": "base64-image-data",
        "src": ""
      }
    ]
  }
}
```

---

## Logging and Monitoring

### Logs

- Logs are managed using Serilog.
  - Logs stored in `Logs/app-log-.txt`.
  - Console logs for real-time debugging.

### Error Handling

- Errors from Shopify are logged for debugging and audit purposes.

---

## Testing

### Test Cases

1. Valid product creation with all fields.
2. Missing mandatory fields.
3. Duplicate SKU validation.
4. Image upload via file and URL.

### Postman Testing

- Test API calls independently using the Shopify REST API.

### UI Testing

- Verify field validations and API responses.

### Edge Cases

- Invalid image file types.
- Large files exceeding allowed limits.

---

## Known Issues and Limitations

- Redirect URI must be whitelisted in Shopify.
- Base64 image size is limited by Shopify’s API constraints.
- Dependency on a reliable internet connection for API calls.

---

## Future Enhancements

- Add bulk product upload functionality.
- Support for additional product fields (tags, inventory, etc.).
- Implement more granular error tracking and notifications.

---

## Conclusion

This application successfully integrates with Shopify’s Admin REST API to manage products efficiently. It is designed for ease of use and extensibility, making it a valuable tool for Shopify store administrators.

