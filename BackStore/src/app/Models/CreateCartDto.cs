// MyApi.Models/CreateCartDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Models
{
    public class CreateCartDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public List<CartProductDto> Products { get; set; } = new List<CartProductDto>();
    }

    public class CartProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}