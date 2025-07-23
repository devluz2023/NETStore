// tests/UnitTests/Services/SaleServiceTests.cs
using Xunit;
using Moq;
using MyApi.Models;
using MyApi.Dtos;
using MyApi.Repositories;
using MyApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyApi.UnitTests.Services
{
    public class SaleServiceTests
    {
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly Mock<ILogger<SaleService>> _mockLogger;
        private readonly SaleService _saleService;

        public SaleServiceTests()
        {
            _mockSaleRepository = new Mock<ISaleRepository>();
            _mockLogger = new Mock<ILogger<SaleService>>();
            _saleService = new SaleService(_mockSaleRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateSaleAsync_ShouldCalculateTotalsAndSaveSale_AndLogEvent()
        {
            // Arrange
            var createDto = new CreateSaleDto
            {
                Date = DateTime.Now,
                Customer = new ExternalCustomerDto { CustomerId = 1, CustomerName = "Test Customer" },
                Branch = new ExternalBranchDto { BranchId = 1, BranchName = "Test Branch" },
                Products = new List<SaleItemDto>
                {
                    new SaleItemDto { ProductId = 1, ProductName = "Item A", Quantity = 5, UnitPrice = 10.00m }, // 10% discount
                    new SaleItemDto { ProductId = 2, ProductName = "Item B", Quantity = 12, UnitPrice = 5.00m } // 20% discount
                }
            };

            // Expected calculations:
            // Item A: 5 * 10 = 50, Discount = 50 * 0.10 = 5, Total = 45
            // Item B: 12 * 5 = 60, Discount = 60 * 0.20 = 12, Total = 48
            // Total Sale: 45 + 48 = 93

            _mockSaleRepository.Setup(r => r.CreateAsync(It.IsAny<Sale>()))
                .ReturnsAsync((Sale sale) =>
                {
                    sale.Id = "new_sale_id"; // Simulate MongoDB generating an ID
                    return sale;
                });

            // Act
            var createdSale = await _saleService.CreateSaleAsync(createDto);

            // Assert
            Assert.NotNull(createdSale);
            Assert.Equal(93.00m, createdSale.TotalSaleAmount);
            Assert.False(createdSale.IsCancelled);
            Assert.Equal(2, createdSale.Products.Count);
            Assert.Equal(45.00m, createdSale.Products[0].TotalItemAmount);
            Assert.Equal(48.00m, createdSale.Products[1].TotalItemAmount);

            _mockSaleRepository.Verify(r => r.CreateAsync(It.IsAny<Sale>()), Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SaleCreated")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateSaleAsync_ShouldThrowException_WhenQuantityExceeds20()
        {
            // Arrange
            var createDto = new CreateSaleDto
            {
                Date = DateTime.Now,
                Customer = new ExternalCustomerDto { CustomerId = 1, CustomerName = "Test Customer" },
                Branch = new ExternalBranchDto { BranchId = 1, BranchName = "Test Branch" },
                Products = new List<SaleItemDto>
                {
                    new SaleItemDto { ProductId = 1, ProductName = "Oversized Item", Quantity = 21, UnitPrice = 10.00m }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _saleService.CreateSaleAsync(createDto));
            Assert.Contains("Cannot sell above 20 identical items", exception.Message); // Ensure message matches the business rule
            _mockSaleRepository.Verify(r => r.CreateAsync(It.IsAny<Sale>()), Times.Never); // Should not try to save
        }

        [Fact]
        public async Task GetSaleByIdAsync_ShouldReturnSale_WhenFound()
        {
            // Arrange
            var saleId = "existing_sale_id";
            var expectedSale = new Sale { Id = saleId, SaleNumber = "S123", TotalSaleAmount = 100 };
            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync(expectedSale);

            // Act
            var result = await _saleService.GetSaleByIdAsync(saleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(saleId, result.Id);
            _mockSaleRepository.Verify(r => r.GetByIdAsync(saleId), Times.Once);
        }

        [Fact]
        public async Task UpdateSaleAsync_ShouldUpdateSaleAndRecalculateTotals_AndLogEvent()
        {
            // Arrange
            var saleId = "existing_sale_id";
            var existingSale = new Sale
            {
                Id = saleId,
                SaleNumber = "S123",
                Date = DateTime.Now.AddDays(-1),
                Customer = new ExternalCustomer { CustomerId = 1, CustomerName = "Old Customer" },
                Branch = new ExternalBranch { BranchId = 1, BranchName = "Old Branch" },
                Products = new List<SaleItem>
                {
                    new SaleItem { ProductId = 1, ProductName = "Old Item", Quantity = 2, UnitPrice = 10.00m }
                }
            };
            existingSale.Products[0].ApplyDiscountRules();
            existingSale.CalculateTotals(); // Initial total: 20

            var updateDto = new UpdateSaleDto
            {
                Date = DateTime.Now,
                Customer = new ExternalCustomerDto { CustomerId = 2, CustomerName = "New Customer" },
                Branch = new ExternalBranchDto { BranchId = 2, BranchName = "New Branch" },
                Products = new List<SaleItemDto>
                {
                    new SaleItemDto { ProductId = 3, ProductName = "New Item C", Quantity = 5, UnitPrice = 20.00m }, // 10% discount
                    new SaleItemDto { ProductId = 4, ProductName = "New Item D", Quantity = 10, UnitPrice = 10.00m } // 20% discount
                }
            };
            // Expected calculations for update:
            // Item C: 5 * 20 = 100, Discount = 100 * 0.10 = 10, Total = 90
            // Item D: 10 * 10 = 100, Discount = 100 * 0.20 = 20, Total = 80
            // Total Sale: 90 + 80 = 170

            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync(existingSale);
            _mockSaleRepository.Setup(r => r.UpdateAsync(It.IsAny<Sale>())).ReturnsAsync(true);

            // Act
            var updatedSale = await _saleService.UpdateSaleAsync(saleId, updateDto);

            // Assert
            Assert.NotNull(updatedSale);
            Assert.Equal(saleId, updatedSale.Id);
            Assert.Equal("New Customer", updatedSale.Customer.CustomerName);
            Assert.Equal(170.00m, updatedSale.TotalSaleAmount);
            Assert.Equal(2, updatedSale.Products.Count);
            Assert.Equal(90.00m, updatedSale.Products[0].TotalItemAmount);
            Assert.Equal(80.00m, updatedSale.Products[1].TotalItemAmount);


            _mockSaleRepository.Verify(r => r.GetByIdAsync(saleId), Times.Once);
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.Is<Sale>(s => s.Id == saleId && s.TotalSaleAmount == 170.00m)), Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SaleModified")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateSaleAsync_ShouldReturnNull_WhenSaleNotFound()
        {
            // Arrange
            var saleId = "non_existent_id";
            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync((Sale)null);

            // Act
            var result = await _saleService.UpdateSaleAsync(saleId, new UpdateSaleDto());

            // Assert
            Assert.Null(result);
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>()), Times.Never);
        }

        [Fact]
        public async Task CancelSaleAsync_ShouldMarkSaleAsCancelled_AndRecalculateTotals_AndLogEvent()
        {
            // Arrange
            var saleId = "sale_to_cancel";
            var existingSale = new Sale
            {
                Id = saleId,
                SaleNumber = "S456",
                IsCancelled = false,
                Products = new List<SaleItem>
                {
                    new SaleItem { ProductId = 1, ProductName = "Item X", Quantity = 5, UnitPrice = 10.00m, IsCancelled = false }
                }
            };
            existingSale.Products[0].ApplyDiscountRules(); // Total 45.00
            existingSale.CalculateTotals(); // Sale total 45.00

            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync(existingSale);
            _mockSaleRepository.Setup(r => r.UpdateAsync(It.IsAny<Sale>())).ReturnsAsync(true);

            // Act
            var result = await _saleService.CancelSaleAsync(saleId);

            // Assert
            Assert.True(result);
            Assert.True(existingSale.IsCancelled);
            Assert.True(existingSale.Products[0].IsCancelled); // Item should also be cancelled
            Assert.Equal(0, existingSale.TotalSaleAmount); // Total sale amount should be 0
            Assert.Equal(0, existingSale.Products[0].TotalItemAmount); // Item total should be 0

            _mockSaleRepository.Verify(r => r.GetByIdAsync(saleId), Times.Once);
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.Is<Sale>(s => s.Id == saleId && s.IsCancelled == true && s.TotalSaleAmount == 0)), Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SaleCancelled")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelSaleAsync_ShouldReturnFalse_WhenSaleNotFoundOrAlreadyCancelled()
        {
            // Arrange
            var notFoundId = "not_found";
            var alreadyCancelledId = "already_cancelled";
            var cancelledSale = new Sale { Id = alreadyCancelledId, IsCancelled = true };

            _mockSaleRepository.Setup(r => r.GetByIdAsync(notFoundId)).ReturnsAsync((Sale)null);
            _mockSaleRepository.Setup(r => r.GetByIdAsync(alreadyCancelledId)).ReturnsAsync(cancelledSale);

            // Act
            var resultNotFound = await _saleService.CancelSaleAsync(notFoundId);
            var resultAlreadyCancelled = await _saleService.CancelSaleAsync(alreadyCancelledId);

            // Assert
            Assert.False(resultNotFound);
            Assert.False(resultAlreadyCancelled);
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSaleAsync_ShouldReturnTrue_WhenSaleDeleted()
        {
            // Arrange
            var saleId = "sale_to_delete";
            _mockSaleRepository.Setup(r => r.DeleteAsync(saleId)).ReturnsAsync(true);

            // Act
            var result = await _saleService.DeleteSaleAsync(saleId);

            // Assert
            Assert.True(result);
            _mockSaleRepository.Verify(r => r.DeleteAsync(saleId), Times.Once);
        }

        [Fact]
        public async Task CancelSaleItemAsync_ShouldCancelSpecificItemAndRecalculateSaleTotal()
        {
            // Arrange
            var saleId = "sale_id_with_items";
            var productIdToCancel = 1;
            var existingSale = new Sale
            {
                Id = saleId,
                SaleNumber = "S789",
                IsCancelled = false,
                Products = new List<SaleItem>
                {
                    new SaleItem { ProductId = productIdToCancel, ProductName = "Item 1", Quantity = 5, UnitPrice = 10.00m, IsCancelled = false }, // 10% disc, total 45
                    new SaleItem { ProductId = 2, ProductName = "Item 2", Quantity = 2, UnitPrice = 20.00m, IsCancelled = false } // No disc, total 40
                }
            };
            // Initial calculation
            existingSale.Products[0].ApplyDiscountRules();
            existingSale.Products[1].ApplyDiscountRules();
            existingSale.CalculateTotals(); // TotalSaleAmount = 45 + 40 = 85

            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync(existingSale);
            _mockSaleRepository.Setup(r => r.UpdateAsync(It.IsAny<Sale>())).ReturnsAsync(true);

            // Act
            var updatedSale = await _saleService.CancelSaleItemAsync(saleId, productIdToCancel);

            // Assert
            Assert.NotNull(updatedSale);
            Assert.True(updatedSale.Products.First(p => p.ProductId == productIdToCancel).IsCancelled);
            Assert.Equal(0, updatedSale.Products.First(p => p.ProductId == productIdToCancel).TotalItemAmount);
            Assert.Equal(40.00m, updatedSale.TotalSaleAmount); // Only Item 2's total remains

            _mockSaleRepository.Verify(r => r.GetByIdAsync(saleId), Times.Once);
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.Is<Sale>(s => s.Id == saleId && s.TotalSaleAmount == 40.00m)), Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("ItemCancelled")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelSaleItemAsync_ShouldReturnNull_WhenSaleNotFoundOrItemNotFound()
        {
            // Arrange
            var saleId = "non_existent_sale";
            var productId = 1;
            var existingSale = new Sale
            {
                Id = "found_sale",
                Products = new List<SaleItem>
                {
                    new SaleItem { ProductId = 99, ProductName = "Other Item", Quantity = 1, UnitPrice = 10 }
                }
            };

            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync((Sale)null); // Sale not found
            _mockSaleRepository.Setup(r => r.GetByIdAsync("found_sale")).ReturnsAsync(existingSale); // Item not found within this sale

            // Act
            var resultNotFoundSale = await _saleService.CancelSaleItemAsync(saleId, productId);
            var resultNotFoundItem = await _saleService.CancelSaleItemAsync("found_sale", productId);

            // Assert
            Assert.Null(resultNotFoundSale);
            Assert.Null(resultNotFoundItem);
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>()), Times.Never);
        }
    }
}