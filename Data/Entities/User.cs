using Data.Base;
using Data.Enums;

namespace Data.Entities;

public class User : AuditableEntity
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public ICollection<UserQuest> UserQuests { get; set; }
        = new List<UserQuest>();

    public ICollection<UserReward> UserRewards { get; set; }
        = new List<UserReward>();
}