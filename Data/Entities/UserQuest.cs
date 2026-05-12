using Data.Base;

namespace Data.Entities;

public class UserQuest : AuditableEntity
{
	public int UserId { get; set; }

	public int QuestId { get; set; }

	public bool IsCompleted { get; set; }

	public int? LastCompletedTaskOrder { get; set; }

	public User User { get; set; } = null!;

	public Quest Quest { get; set; } = null!;
}