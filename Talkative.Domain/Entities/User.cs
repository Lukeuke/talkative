﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotChocolate;

namespace Talkative.Domain.Entities;

public class User
{
    [Key]
    public required Guid Id { get; set; }

    [MaxLength(25)]
    public required string Username { get; set; }
    
    [MaxLength(25)]
    public required string FirstName { get; set; }
    
    [MaxLength(50)]
    public required string LastName { get; set; }
    public required string Email { get; set; }

    [GraphQLIgnore] public required string PasswordHash { get; set; }
    [GraphQLIgnore] public string Salt { get; set; } = null!;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    public string? ImageUrl { get; set; }

    public required long CreatedAt { get; set; }
    public bool OnlineStatus { get; set; } = false;
    
    public virtual List<Room> Rooms { get; set; } = null!;
    public virtual List<Message> Messages { get; set; } = null!;
    public virtual List<Invite> Invites { get; set; } = null!;
    public virtual List<RoomReadStatus> RoomReadStatus { get; set; } = null!;
    public ICollection<UserDevice> Devices { get; set; } = null!;
}