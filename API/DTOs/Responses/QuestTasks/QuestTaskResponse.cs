namespace API.DTOs.Responses.QuestTasks;

public class QuestTaskResponse
{
	public int Id { get; set; }

	public string Title { get; set; }
		= string.Empty;

	public string Description { get; set; }
		= string.Empty;

	public int Order { get; set; }
}