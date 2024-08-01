using LaPizzeria.Models.DTO;
using LaPizzeria.Models;
using Microsoft.EntityFrameworkCore;
using LaPizzeria.Services.Interfaces;

namespace LaPizzeria.Services
{
    public class ProductService : IProductService
    {
        private readonly InFornoDbContext _context;

        public ProductService(InFornoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Ingredients).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Ingredients).FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddProductAsync(ProductDTO productDto, byte[] imageBytes, List<int> selectedIngredients)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                ProductImage = imageBytes,
                Description = productDto.Description,
                ProductPrice = productDto.ProductPrice,
                ProductDeliveryTime = productDto.ProductDeliveryTime,
                Ingredients = new List<Ingredient>()
            };

            if (selectedIngredients != null && selectedIngredients.Any())
            {
                foreach (var ingredientId in selectedIngredients)
                {
                    var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                    if (ingredient != null)
                    {
                        product.Ingredients.Add(ingredient);
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(int id, Product product, byte[] imageBytes, List<int> selectedIngredients)
        {
            var productToUpdate = await _context.Products.Include(p => p.Ingredients).FirstOrDefaultAsync(p => p.ProductId == id);

            if (productToUpdate == null)
            {
                throw new KeyNotFoundException();
            }

            if (imageBytes != null && imageBytes.Length > 0)
            {
                productToUpdate.ProductImage = imageBytes;
            }
            else if (productToUpdate.ProductImage == null)
            {
                throw new ArgumentException("ProductImage is required.");
            }

            productToUpdate.ProductName = product.ProductName;
            productToUpdate.Description = product.Description;
            productToUpdate.ProductPrice = product.ProductPrice;
            productToUpdate.ProductDeliveryTime = product.ProductDeliveryTime;
            productToUpdate.Ingredients.Clear();

            if (selectedIngredients != null && selectedIngredients.Any())
            {
                foreach (var ingredientId in selectedIngredients)
                {
                    var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                    if (ingredient != null)
                    {
                        productToUpdate.Ingredients.Add(ingredient);
                    }
                }
            }

            _context.Products.Update(productToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var productToDelete = await _context.Products.Include(p => p.Ingredients).FirstOrDefaultAsync(p => p.ProductId == id);

            if (productToDelete == null)
            {
                throw new KeyNotFoundException();
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderDTO>> GetAllOrderAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductId = oi.Product.ProductId,
                    ProductName = oi.Product.ProductName,
                    ProductPrice = oi.Product.ProductPrice,
                    Quantity = oi.Quantity
                }).ToList(),
                OrderId = o.OrderId,
                Address = o.Address,
                Note = o.Note,
                User = o.User,
                DateTime = o.DateTime,
                isPaid = o.isPaid
            }).ToList();

            return orderDTOs;
        }

        public async Task OrderIsPaidAsync(int orderId)
            {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException();
            }

            order.isPaid = true;
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalPaidOrdersAsync(DateTime date)
        {
            return await _context.Orders
                .CountAsync(o => o.isPaid && o.DateTime.Date == date.Date);
        }

        public async Task<decimal> GetTotalIncomeAsync(DateTime date)
        {
            return await _context.Orders
                .Where(o => o.isPaid && o.DateTime.Date == date.Date)
                .SelectMany(o => o.OrderItems)
                .SumAsync(oi => oi.Quantity * oi.Product.ProductPrice);
        }
    }
}
