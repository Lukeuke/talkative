namespace Talkative.Domain.Models;

public class RoomStatus
{
    public string RoomName { get; set; } = null!;
    public List<UserStatus>? UserStatus { get; set; }
}