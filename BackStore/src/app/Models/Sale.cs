using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApi.Models
{
    public class Sale
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!; // MongoDB generated ID

        public string SaleNumber { get; set; } = Guid.NewGuid().ToString("N"); // Unique per sale
        public DateTime Date { get; set; }

        public ExternalCustomer Customer { get; set; } = new ExternalCustomer();
        public ExternalBranch Branch { get; set; } = new ExternalBranch();

        public List<SaleItem> Products { get; set; } = new List<SaleItem>();

        public decimal TotalSaleAmount { get; private set; }
        public bool IsCancelled { get; set; } = false;

        public void CalculateTotals()
        {
            TotalSaleAmount = Products.Sum(item => item.TotalItemAmount);
        }

        public void CancelSale()
        {
            IsCancelled = true;
            // Optionally, cancel all items within the sale
            foreach (var item in Products)
            {
                item.IsCancelled = true;
                item.CalculateTotalItemAmount(); // Recalculate amount after cancellation
            }
            CalculateTotals(); // Recalculate total sale amount
        }
    }

    public class ExternalCustomer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }

    public class ExternalBranch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }
}