﻿using System.ComponentModel.DataAnnotations;

namespace LaPizzeria.Models.DTO
{
    public class ProductDTO
    {
        public string ProductName { get; set; }

        public byte[] ProductImage { get; set; }


        public string Description { get; set; }


        public decimal ProductPrice { get; set; }


        public int ProductDeliveryTime { get; set; }

        public List<IngredientDTO> Ingredients { get; set; } = new List<IngredientDTO>();

        public string ProductImageUrl => ProductImage != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(ProductImage)}" : null;
    }
}
