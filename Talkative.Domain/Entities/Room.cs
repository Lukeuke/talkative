using System.ComponentModel.DataAnnotations;

namespace Talkative.Domain.Entities;

public class Room
{
    [Key]
    public required Guid Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }

    public required Guid OwnerId { get; set; }

    [UseFiltering]
    [UseSorting]
    public virtual List<User> Users { get; set; } = null!;
    
    [UseFiltering]
    [UseSorting]
    public virtual List<Message> Messages { get; set; } = null!;
}