using System.ComponentModel.DataAnnotations;

namespace Talkative.Domain.Entities;

public class Message
{
    [Key]
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public long CreatedAt { get; set; }
    public long EditedAt { get; set; }

    public virtual User Sender { get; set; }
    public virtual Guid SenderId { get; set; }

    public virtual Room Room { get; set; }
    public virtual Guid RoomId { get; set; }
}