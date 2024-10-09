using System.ComponentModel.DataAnnotations;

namespace Talkative.Domain.Entities;

public class RoomReadStatus
{
    [Key]
    public Guid Id { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Guid UserId { get; set; }

    public virtual Room Room { get; set; } = null!;
    public virtual Guid RoomId { get; set; }
    
    public virtual Message LastReadMessage { get; set; } = null!;
    public virtual Guid LastReadMessageId { get; set; }

    public long UpdatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}