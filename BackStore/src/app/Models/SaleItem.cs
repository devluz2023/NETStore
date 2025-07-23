using System;

namespace MyApi.Models
{
    public class SaleItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; private set; } // Calculated based on rules
        public decimal TotalItemAmount { get; private set; }
        public bool IsCancelled { get; set; } = false;

        public void ApplyDiscountRules()
        {
            if (IsCancelled)
            {
                Discount = Quantity * UnitPrice; // Full discount if cancelled
                TotalItemAmount = 0;
                return;
            }

            // No discount for quantities below 4
            if (Quantity < 4)
            {
                Discount = 0;
            }
            // 20% discount for 10-20 items
            else if (Quantity >= 10 && Quantity <= 20)
            {
                Discount = UnitPrice * Quantity * 0.20m;
            }
            // 10% discount for 4-9 items (this naturally falls after the 10-20 check)
            else if (Quantity >= 4)
            {
                Discount = UnitPrice * Quantity * 0.10m;
            }

            CalculateTotalItemAmount();
        }

        public void CalculateTotalItemAmount()
        {
            if (IsCancelled)
            {
                TotalItemAmount = 0;
            }
            else
            {
                TotalItemAmount = (Quantity * UnitPrice) - Discount;
            }
        }

        public void CancelItem()
        {
            IsCancelled = true;
            ApplyDiscountRules(); // Recalculate total (should become 0)
        }
    }
}