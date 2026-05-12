using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace API.DTOs.Requests.Quests;

public class UpdateQuestRequest
{
	[Required]
	[StringLength(100, MinimumLength = 3)]
	public string Title { get; set; } = string.Empty;

	public int? RewardId { get; set; }

	[Required]
	[StringLength(500)]
	public string Description { get; set; } = string.Empty;


	[Required]
	public QuestDifficulty Difficulty { get; set; }
}