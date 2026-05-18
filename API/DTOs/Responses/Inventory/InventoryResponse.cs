namespace API.DTOs.Responses.UserRewards;

public class InventoryResponse
{
	public int Id { get; set; }

	public int RewardId { get; set; }

	public DateTime ReceivedAt { get; set; }

	public bool IsClaimed { get; set; }
}