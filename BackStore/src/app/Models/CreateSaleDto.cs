using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Dtos
{
    public class CreateSaleDto
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public ExternalCustomerDto Customer { get; set; } = new ExternalCustomerDto();

        [Required]
        public ExternalBranchDto Branch { get; set; } = new ExternalBranchDto();

        [Required]
        [MinLength(1, ErrorMessage = "A sale must have at least one product.")]
        public List<SaleItemDto> Products { get; set; } = new List<SaleItemDto>();
    }

    public class ExternalCustomerDto
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string CustomerName { get; set; } = string.Empty;
    }

    public class ExternalBranchDto
    {
        [Required]
        public int BranchId { get; set; }
        [Required]
        public string BranchName { get; set; } = string.Empty;
    }

    public class SaleItemDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        [Range(1, 20, ErrorMessage = "Quantity must be between 1 and 20.")]
        public int Quantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        public decimal UnitPrice { get; set; }
    }
}