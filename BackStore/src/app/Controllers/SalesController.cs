using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Models;
using MyApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly SaleService _saleService;

        public SalesController(SaleService saleService)
        {
            _saleService = saleService;
        }

        /// <summary>
        /// Retrieves a list of all sales with pagination and sorting.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSales([FromQuery] int _page = 1, [FromQuery] int _size = 10, [FromQuery] string _order = null)
        {
            // You would typically get total items from repository and return pagination details here
        var (sales, totalItems) = await _saleService.GetAllSalesWithCountAsync(_page, _size, _order);

            // Calculate totalPages based on the totalItems obtained from the service
            var totalPages = (int)Math.Ceiling(totalItems / (double)_size);
            if (totalItems == 0) // Handle case where there are no items to prevent division by zero for totalPages if _size is not 0
            {
                totalPages = 0;
            }
            else if (totalPages == 0 && totalItems > 0) // Ensure at least 1 page if items exist
            {
                totalPages = 1;
            }

            return Ok(new
            {
                data = sales,
                totalItems = totalItems,
                currentPage = _page,
                totalPages = totalPages
            });
        }

        /// <summary>
        /// Retrieves a specific sale by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound(new { message = "Sale not found" });
            return Ok(sale);
        }

        /// <summary>
        /// Creates a new sale.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleDto input)
        {
            try
            {
                var sale = await _saleService.CreateSaleAsync(input);
                return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing sale.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateSaleDto input)
        {
            try
            {
                var updatedSale = await _saleService.UpdateSaleAsync(id, input);
                if (updatedSale == null)
                    return NotFound(new { message = "Sale not found" });
                return Ok(updatedSale);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancels a sale.
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelSale(string id)
        {
            var result = await _saleService.CancelSaleAsync(id);
            if (!result)
                return NotFound(new { message = "Sale not found or already cancelled" });
            return Ok(new { message = "Sale cancelled successfully" });
        }

        /// <summary>
        /// Cancels a specific item within a sale.
        /// </summary>
        [HttpPut("{saleId}/items/{productId}/cancel")]
        public async Task<IActionResult> CancelSaleItem(string saleId, int productId)
        {
            var updatedSale = await _saleService.CancelSaleItemAsync(saleId, productId);
            if (updatedSale == null)
                return NotFound(new { message = "Sale or product item not found, or sale already cancelled" });
            return Ok(updatedSale);
        }

        /// <summary>
        /// Deletes a sale.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _saleService.DeleteSaleAsync(id);
            if (!result)
                return NotFound(new { message = "Sale not found" });
            return Ok(new { message = "Sale deleted successfully" });
        }
    }
}