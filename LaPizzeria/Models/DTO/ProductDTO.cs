using System.ComponentModel.DataAnnotations;

namespace LaPizzeria.Models.DTO
{
    public class ProductDTO
    {
        public string ProductName { get; set; }

        public byte[] ProductImage { get; set; }

   
        public string Description { get; set; }

       
        public decimal ProductPrice { get; set; }

       
        public int ProductDeliveryTime { get; set; }

        public List<Ingredient> Ingredients { get; set; } = [];
    }
}
