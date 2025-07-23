using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Dtos
{
    public class UpdateSaleDto
    {
        // ID is typically in the URL for PUT, but might be useful here for consistency
        // public string Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public ExternalCustomerDto Customer { get; set; } = new ExternalCustomerDto();

        [Required]
        public ExternalBranchDto Branch { get; set; } = new ExternalBranchDto();

        [Required]
        [MinLength(1, ErrorMessage = "A sale must have at least one product.")]
        public List<SaleItemDto> Products { get; set; } = new List<SaleItemDto>();
        
        // You might have properties to indicate status changes (e.g., IsCancelled) in a dedicated DTO or endpoint
    }

    // Reuse ExternalCustomerDto, ExternalBranchDto, SaleItemDto from CreateSaleDto if identical
}