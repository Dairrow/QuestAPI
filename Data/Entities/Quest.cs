using Data.Base;
using Data.Enums;

namespace Data.Entities;

public class Quest : AuditableEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public QuestDifficulty Difficulty { get; set; }


    public ICollection<QuestTask> Tasks { get; set; }
        = new List<QuestTask>();

    public ICollection<UserQuest> UserQuests { get; set; }
        = new List<UserQuest>();
}