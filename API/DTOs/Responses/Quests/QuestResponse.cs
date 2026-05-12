using Data.Enums;

namespace API.DTOs.Responses.Quests;

public class QuestResponse
{
	public int Id { get; set; }

	public string Title { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public QuestDifficulty Difficulty { get; set; }

	public int? RewardId { get; set; }

	public DateTime CreatedAt { get; set; }
}