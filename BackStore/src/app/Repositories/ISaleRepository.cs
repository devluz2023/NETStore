using MyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApi.Repositories
{
    public interface ISaleRepository
    {
        Task<Sale> CreateAsync(Sale sale);
        Task<Sale?> GetByIdAsync(string id);
        Task<IEnumerable<Sale>> GetAllAsync(int page, int size, string orderBy);
        Task<bool> UpdateAsync(Sale sale);
        Task<bool> DeleteAsync(string id);
        Task<long> CountAsync(); // For pagination
    }
}
