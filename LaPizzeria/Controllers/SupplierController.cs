using LaPizzeria.Models;
using Microsoft.AspNetCore.Mvc;
using LaPizzeria.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace LaPizzeria.Controllers
{
    public class SupplierController : Controller
    {
        private readonly InFornoDbContext _context;

        public SupplierController(InFornoDbContext context)
        {
            _context = context;
        }
        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Ingredients()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind("ProductName,ProductImage,Description,ProductPrice,ProductDeliveryTime,Ingredients")] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                ProductImage = productDto.ProductImage,
                Description = productDto.Description,
                ProductPrice = productDto.ProductPrice,
                ProductDeliveryTime = productDto.ProductDeliveryTime,
                Ingredients = productDto.Ingredients.Select(i => new Ingredient
                {
                    IngredientName = i.IngredientName
                }).ToList()
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct([Bind("ProductName,ProductImage,Description,ProductPrice,ProductDeliveryTime,Ingredients")] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            var productToUpdate = await _context.Products
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.ProductName == productDto.ProductName);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.ProductImage = productDto.ProductImage;
            productToUpdate.Description = productDto.Description;
            productToUpdate.ProductPrice = productDto.ProductPrice;
            productToUpdate.ProductDeliveryTime = productDto.ProductDeliveryTime;
            productToUpdate.Ingredients = productDto.Ingredients.Select(i => new Ingredient
            {
                IngredientName = i.IngredientName
            }).ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct([Bind("ProductName")] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            var productToDelete = await _context.Products
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.ProductName == productDto.ProductName);

            if (productToDelete == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIngredient([Bind("IngredientName")] IngredientDTO ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredientDto);
            }

            var ingredient = new Ingredient
            {
                IngredientName = ingredientDto.IngredientName
            };

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredient");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIngredient([Bind("IngredientName")] IngredientDTO ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredientDto);
            }

            var ingredientToUpdate = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.IngredientName == ingredientDto.IngredientName);

            if (ingredientToUpdate == null)
            {
                return NotFound();
            }

            ingredientToUpdate.IngredientName = ingredientDto.IngredientName;
            _context.Ingredients.Update(ingredientToUpdate);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredient");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIngredient([Bind("IngredientName")] IngredientDTO ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredientDto);
            }

            var ingredientToDelete = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.IngredientName == ingredientDto.IngredientName);

            if (ingredientToDelete == null)
            {
                return NotFound();
            }

            _context.Ingredients.Remove(ingredientToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredient");
        }


    }
}
