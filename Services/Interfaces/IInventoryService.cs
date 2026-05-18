using Data.Entities;

namespace Services.Interfaces;

public interface IInventoryService
{
	Task<IReadOnlyCollection<Inventory>>
		GetByUserIdAsync(
			int userId,
			CancellationToken cancellationToken = default);

	Task<Inventory>
		GetByIdAsync(
			int id,
			CancellationToken cancellationToken = default);

	Task<Inventory>
		CreateAsync(
			Inventory entity,
			CancellationToken cancellationToken = default);

	Task<Inventory> 
		ClaimAsync(int userRewardId, 
		CancellationToken cancellationToken = default);

	Task DeleteAsync(
		int id,
		CancellationToken cancellationToken = default);
}