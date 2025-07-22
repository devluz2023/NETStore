
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Models
{
    public class UpdateCartDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public List<CartProductDto> Products { get; set; } = new List<CartProductDto>();
    }
}