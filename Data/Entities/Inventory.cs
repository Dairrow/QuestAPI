using Data.Base;

namespace Data.Entities;

public class Inventory : AuditableEntity
{
	public int UserId { get; set; }

	public int RewardId { get; set; }

	public DateTime ReceivedAt { get; set; }

	public bool IsClaimed { get; set; }

	public User User { get; set; } = null!;

	public Reward Reward { get; set; } = null!;
}