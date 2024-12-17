using Microsoft.AspNetCore.Mvc;
using ShopifyTool.Models;
using ShopifyTool.Services;

namespace ShopifyTool.Controllers
{
    public class ShopifyController : Controller
    {
        private readonly ShopifyService _service;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public ShopifyController(ShopifyService service, IConfiguration configuration, TokenService tokenService)
        {
            _service = service;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Determine if the token exists
            var isAuthorized = !string.IsNullOrEmpty(_tokenService.AccessToken);

            // Pass the authorization status to the view
            ViewBag.IsAuthorized = isAuthorized;

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ProductDTO productDTO = new()
            {
                Variants = [new()],
                Images = [new()]
            };
            return View(productDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO product, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                //Return to the view with validation errors.
                return View(product);
            }

            // Process the uploaded file if it exists.
            if (imageFile != null && imageFile.Length > 0 && string.IsNullOrEmpty(product.Images[0].Src))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    product.Images[0].Attachment = Convert.ToBase64String(fileBytes);
                }
            }

            var response = await _service.CreateProductAsync(product);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                //Extract and display validation errors from response.
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.Error = errorContent;
                return View(product);
            }
            else if (!response.IsSuccessStatusCode)
            {
                //Handle the general API errors.
                ViewBag.Error = "Failed to create the product. Please try again.";
                return View(product);
            }

            //Redirect success response.
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction("Success");
        }

        //Success Page
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}