namespace Talkative.Domain.Models;

public class UserStatus
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public bool IsOnline { get; set; }
}