using Xunit;
using MyApi.Models; // Ensure this namespace matches your models

namespace MyApi.UnitTests.Models
{
    public class SaleItemTests
    {
        [Theory]
        [InlineData(1, 10.00, 0, 10.00)] // Below 4 items, no discount
        [InlineData(3, 10.00, 0, 30.00)] // Below 4 items, no discount
        [InlineData(4, 10.00, 4.00, 36.00)] // 4 items, 10% discount (4 * 10 * 0.10 = 4)
        [InlineData(9, 10.00, 9.00, 81.00)] // 9 items, 10% discount (9 * 10 * 0.10 = 9)
        [InlineData(10, 10.00, 20.00, 80.00)] // 10 items, 20% discount (10 * 10 * 0.20 = 20)
        [InlineData(15, 10.00, 30.00, 120.00)] // 15 items, 20% discount (15 * 10 * 0.20 = 30)
        [InlineData(20, 10.00, 40.00, 160.00)] // 20 items, 20% discount (20 * 10 * 0.20 = 40)
        public void ApplyDiscountRules_ShouldCalculateCorrectDiscountAndTotal(int quantity, decimal unitPrice, decimal expectedDiscount, decimal expectedTotalItemAmount)
        {
            // Arrange
            var item = new SaleItem
            {
                ProductId = 1,
                ProductName = "Test Product",
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            // Act
            item.ApplyDiscountRules();

            // Assert
            Assert.Equal(expectedDiscount, item.Discount);
            Assert.Equal(expectedTotalItemAmount, item.TotalItemAmount);
        }

        [Fact]
        public void CancelItem_ShouldSetIsCancelledToTrueAndZeroOutTotal()
        {
            // Arrange
            var item = new SaleItem
            {
                ProductId = 1,
                ProductName = "Test Product",
                Quantity = 5,
                UnitPrice = 10.00m
            };
            item.ApplyDiscountRules(); // Initial calculation

            // Act
            item.CancelItem();

            // Assert
            Assert.True(item.IsCancelled);
            Assert.Equal(0, item.TotalItemAmount);
            Assert.Equal(50.00m, item.Discount); // Should discount full amount if cancelled
        }
    }
}