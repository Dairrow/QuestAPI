namespace API.DTOs.Responses.UserQuests;

public class UserQuestResponse
{
	public int Id { get; set; }

	public int QuestId { get; set; }

	public int? LastCompletedTaskOrder { get; set; }

	public bool IsCompleted { get; set; }
}