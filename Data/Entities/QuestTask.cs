using Data.Base;

namespace Data.Entities;

public class QuestTask : AuditableEntity
{
	public string Title { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public int QuestId { get; set; }

	public Quest Quest { get; set; } = null!;
}