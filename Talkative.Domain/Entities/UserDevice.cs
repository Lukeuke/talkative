using System.ComponentModel.DataAnnotations;

namespace Talkative.Domain.Entities;

public class UserDevice
{
    [Key]
    public Guid Id { get; set; }
    
    public virtual Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public string ExpoPushToken { get; set; } = null!;
    public DateTime TokenUpdatedAt { get; set; }
    public bool IsActive { get; set; }
}