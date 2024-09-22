using System.ComponentModel.DataAnnotations;

namespace Talkative.Domain.Entities;

public class Invite
{
    [Key]
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    public virtual Guid UserId { get; set; }
}