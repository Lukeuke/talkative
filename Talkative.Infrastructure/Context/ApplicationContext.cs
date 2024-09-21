using Microsoft.EntityFrameworkCore;
using Talkative.Domain.Entities;

namespace Talkative.Infrastructure.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasMany(x => x.Messages);
        modelBuilder.Entity<User>().HasMany(x => x.Rooms);

        modelBuilder.Entity<Room>().HasMany(x => x.Messages);
        modelBuilder.Entity<Room>().HasMany(x => x.Users);
        
        modelBuilder.Entity<Message>().HasOne(x => x.Room);
        modelBuilder.Entity<Message>().HasOne(x => x.Sender);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Message> Messages { get; set; }
}