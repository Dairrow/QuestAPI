using Data.Base;
using Data.Enums;

namespace Data.Entities;

public class Reward : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public RewardType Type { get; set; }

    public int Value { get; set; }

    public ICollection<UserReward> UserRewards { get; set; }
        = new List<UserReward>();
}