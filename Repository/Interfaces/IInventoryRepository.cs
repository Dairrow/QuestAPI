using Data.Entities;

namespace Repository.Interfaces;

public interface IInventoryRepository : IBaseRepository<Inventory>
{
	Task<IReadOnlyCollection<Inventory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}