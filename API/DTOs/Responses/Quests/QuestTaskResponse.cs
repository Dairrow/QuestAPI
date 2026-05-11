namespace API.DTOs.Responses.Quests;

public class QuestTaskResponse
{
	public int Id { get; set; }

	public string Title { get; set; } = string.Empty;

	public bool IsRequired { get; set; }
}