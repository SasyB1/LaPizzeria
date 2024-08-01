using LaPizzeria.Models.DTO;
using LaPizzeria.Models;

namespace LaPizzeria.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductDTO productDto, byte[] imageBytes, List<int> selectedIngredients);
        Task UpdateProductAsync(int id, Product product, byte[] imageBytes, List<int> selectedIngredients);
        Task DeleteProductAsync(int id);
        Task<List<OrderDTO>> GetAllOrderAsync();
        Task OrderIsPaidAsync(int orderId);
        Task<int> GetTotalPaidOrdersAsync(DateTime date);
        Task<decimal> GetTotalIncomeAsync(DateTime date);
    }
}
