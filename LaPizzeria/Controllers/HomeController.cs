using LaPizzeria.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LaPizzeria.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LaPizzeria.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = Policies.SupplierOrCustomer)]
        public async Task<IActionResult> Catalogo()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }
        [Authorize(Policy = Policies.SupplierOrCustomer)]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return View(product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
