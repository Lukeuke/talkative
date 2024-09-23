using Newtonsoft.Json;

namespace Talkative.Domain.Models;

public class UserStatus
{
    public required Guid UserId { get; set; }
    public required bool IsOnline { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}