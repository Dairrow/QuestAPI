using Data.Base;

namespace Data.Entities;

public class RefreshToken : AuditableEntity
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}