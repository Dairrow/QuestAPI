using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.QuestTasks;

public class CreateQuestTaskRequest
{
	[Required]
	public string Title { get; set; }
		= string.Empty;

	public string Description { get; set; }
		= string.Empty;

	[Range(1, int.MaxValue)]
	public int Order { get; set; }
}