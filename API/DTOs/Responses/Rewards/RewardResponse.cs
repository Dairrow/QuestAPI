using Data.Enums;

namespace API.DTOs.Responses.Rewards;

public class RewardResponse
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public RewardType Type { get; set; }

	public int Value { get; set; }
}