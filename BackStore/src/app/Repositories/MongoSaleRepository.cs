using MongoDB.Driver;
using MongoDB.Driver.Linq; // For AsQueryable()
using MyApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi.Repositories
{
    public class MongoSaleRepository : ISaleRepository
    {
        private readonly IMongoCollection<Sale> _salesCollection;

        public MongoSaleRepository(MongoDbContext mongoContext)
        {
            _salesCollection = mongoContext.Sales; // Assuming you add Sales collection to MongoDbContext
        }

        public async Task<Sale> CreateAsync(Sale sale)
        {
            await _salesCollection.InsertOneAsync(sale);
            return sale;
        }

        public async Task<Sale?> GetByIdAsync(string id)
        {
            return await _salesCollection.Find(sale => sale.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Sale>> GetAllAsync(int page, int size, string orderBy)
        {
            var queryable = _salesCollection.AsQueryable();

            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderParts = orderBy.Split(',');
                foreach (var part in orderParts)
                {
                    var fieldAndDirection = part.Trim().Split(' ');
                    var field = fieldAndDirection[0].ToLower();
                    var direction = fieldAndDirection.Length > 1 && fieldAndDirection[1].ToLower() == "desc" ? -1 : 1;

                    // Apply sorting dynamically
                    // MongoDB.Driver.Linq usually handles standard LINQ OrderBy/OrderByDescending
                    switch (field)
                    {
                        case "id":
                            queryable = direction == -1 ? queryable.OrderByDescending(s => s.Id) : queryable.OrderBy(s => s.Id);
                            break;
                        case "salenumber":
                            queryable = direction == -1 ? queryable.OrderByDescending(s => s.SaleNumber) : queryable.OrderBy(s => s.SaleNumber);
                            break;
                        case "date":
                            queryable = direction == -1 ? queryable.OrderByDescending(s => s.Date) : queryable.OrderBy(s => s.Date);
                            break;
                        case "totalsaleamount":
                            queryable = direction == -1 ? queryable.OrderByDescending(s => s.TotalSaleAmount) : queryable.OrderBy(s => s.TotalSaleAmount);
                            break;
                        // Add other sortable fields as needed
                        default:
                            break;
                    }
                }
            }
            else
            {
                queryable = queryable.OrderBy(s => s.Id); // Default sort
            }

            return await queryable
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Sale sale)
        {
            var result = await _salesCollection.ReplaceOneAsync(s => s.Id == sale.Id, sale);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _salesCollection.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<long> CountAsync()
        {
            return await _salesCollection.CountDocumentsAsync(_ => true);
        }
    }
}