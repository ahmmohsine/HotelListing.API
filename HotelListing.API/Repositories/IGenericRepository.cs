using System.Linq.Expressions;

namespace HotelListing.API.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);

        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    }
}
