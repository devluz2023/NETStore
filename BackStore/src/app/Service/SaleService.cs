using MyApi.Models;
using MyApi.Dtos;
using MyApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // For logging events

namespace MyApi.Services
{
    public class SaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SaleService> _logger; // For event logging

        public SaleService(ISaleRepository saleRepository, ILogger<SaleService> logger)
        {
            _saleRepository = saleRepository;
            _logger = logger;
        }

        public async Task<Sale> CreateSaleAsync(CreateSaleDto createDto)
        {
            // Map DTO to Sale entity
            var sale = new Sale
            {
                Date = createDto.Date.ToUniversalTime(),
                Customer = new ExternalCustomer { CustomerId = createDto.Customer.CustomerId, CustomerName = createDto.Customer.CustomerName },
                Branch = new ExternalBranch { BranchId = createDto.Branch.BranchId, BranchName = createDto.Branch.BranchName },
                Products = createDto.Products.Select(dto => new SaleItem
                {
                    ProductId = dto.ProductId,
                    ProductName = dto.ProductName,
                    Quantity = dto.Quantity,
                    UnitPrice = dto.UnitPrice
                }).ToList()
            };

            // Apply business rules and calculate totals for each item
            foreach (var item in sale.Products)
            {
                // Validate quantity restriction
                if (item.Quantity > 20)
                {
                    throw new ArgumentException($"Cannot sell more than 20 units of product {item.ProductName} (ID: {item.ProductId}).");
                }
                item.ApplyDiscountRules();
            }

            // Calculate total for the sale
            sale.CalculateTotals();

            await _saleRepository.CreateAsync(sale);

            _logger.LogInformation($"Event: SaleCreated - SaleNumber: {sale.SaleNumber}, Id: {sale.Id}");
            // In a real app, you'd publish a SaleCreatedEvent here.

            return sale;
        }

        public async Task<Sale?> GetSaleByIdAsync(string id)
        {
            return await _saleRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync(int page, int size, string orderBy)
        {
            // This method would call repository with pagination/sorting logic
            return await _saleRepository.GetAllAsync(page, size, orderBy);
        }


        public async Task<Sale> UpdateSaleAsync(string id, UpdateSaleDto updateDto)
        {
            var existingSale = await _saleRepository.GetByIdAsync(id);
            if (existingSale == null)
            {
                return null; // Or throw NotFoundException
            }

            // Update properties
            existingSale.Date = updateDto.Date.ToUniversalTime();
            existingSale.Customer = new ExternalCustomer { CustomerId = updateDto.Customer.CustomerId, CustomerName = updateDto.Customer.CustomerName };
            existingSale.Branch = new ExternalBranch { BranchId = updateDto.Branch.BranchId, BranchName = updateDto.Branch.BranchName };

            // Handle product updates (simplified for now - full replacement)
            // In a real-world scenario, you might need more granular item-level updates
            existingSale.Products.Clear();
            existingSale.Products.AddRange(updateDto.Products.Select(dto => new SaleItem
            {
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            }));

            // Re-apply business rules and recalculate totals for each item and the sale
            foreach (var item in existingSale.Products)
            {
                 if (item.Quantity > 20)
                {
                    throw new ArgumentException($"Cannot sell more than 20 units of product {item.ProductName} (ID: {item.ProductId}).");
                }
                item.ApplyDiscountRules();
            }
            existingSale.CalculateTotals();

            await _saleRepository.UpdateAsync(existingSale);

            _logger.LogInformation($"Event: SaleModified - SaleNumber: {existingSale.SaleNumber}, Id: {existingSale.Id}");
            // Publish SaleModifiedEvent

            return existingSale;
        }

        public async Task<bool> CancelSaleAsync(string id)
        {
            var existingSale = await _saleRepository.GetByIdAsync(id);
            if (existingSale == null || existingSale.IsCancelled)
            {
                return false;
            }

            existingSale.CancelSale(); // Mark sale and its items as cancelled
            await _saleRepository.UpdateAsync(existingSale);

            _logger.LogInformation($"Event: SaleCancelled - SaleNumber: {existingSale.SaleNumber}, Id: {existingSale.Id}");
            // Publish SaleCancelledEvent

            return true;
        }

        // Optional: Method to cancel a specific item within a sale
        public async Task<Sale?> CancelSaleItemAsync(string saleId, int productId)
        {
            var existingSale = await _saleRepository.GetByIdAsync(saleId);
            if (existingSale == null || existingSale.IsCancelled)
            {
                return null;
            }

            var itemToCancel = existingSale.Products.FirstOrDefault(p => p.ProductId == productId && !p.IsCancelled);
            if (itemToCancel == null)
            {
                return null; // Item not found or already cancelled
            }

            itemToCancel.CancelItem();
            existingSale.CalculateTotals(); // Recalculate sale total

            await _saleRepository.UpdateAsync(existingSale);

            _logger.LogInformation($"Event: ItemCancelled - SaleNumber: {existingSale.SaleNumber}, Item ProductId: {productId}");
            // Publish ItemCancelledEvent

            return existingSale;
        }
 public async Task<(IEnumerable<Sale> Sales, long TotalCount)> GetAllSalesWithCountAsync(int page, int size, string orderBy)
        {
            var sales = await _saleRepository.GetAllAsync(page, size, orderBy);
            var totalCount = await _saleRepository.CountAsync();
            return (sales, totalCount);
        }

        public async Task<bool> DeleteSaleAsync(string id)
        {
            return await _saleRepository.DeleteAsync(id);
        }
    }
}