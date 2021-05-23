using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Signaler.Data
{
    public class SignalerDbContext : DbContext
    {
        public SignalerDbContext(DbContextOptions<SignalerDbContext> contextOptions)
            : base(contextOptions)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Username)
                .HasName("username");

            modelBuilder.Entity<User>()
                 .HasMany(u => u.Groups)
                 .WithMany(g => g.Users)
                 .UsingEntity<Dictionary<string, object>>("BelongsTo",
                 j => j.HasOne<Group>().WithMany().HasForeignKey("GroupId"),
                 j => j.HasOne<User>().WithMany().HasForeignKey("Username"));

            modelBuilder.Entity<User>()
                 .HasMany(u => u.Messages)
                 .WithOne(m => m.User)
                 .HasForeignKey("senderUsername");

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Messages)
                .WithOne(m => m.Group);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.UserCreator)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey("creator");
            
        }

        public DbSet<User> User { get; set; }
        public DbSet<Message> Message { get; set; }

        public DbSet<Group> Group { get; set; }
    }

    public class User
    {
        public User()
        {
            Messages = new LinkedList<Message>();
            Groups = new LinkedList<Group>();
        }
        public string Username { get; set; }
        public string Password { get; set; }

        public ICollection<Group> Groups { get; set; }

        public ICollection<Group> CreatedGroups { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }

    public class Group
    {
        public Group()
        {
            Users = new LinkedList<User>();
            Messages = new LinkedList<Message>();
        }
        public int GroupId { get; set; }

        
        // Walues can be Group or Chat
        public string Type { get; set; }

        public User UserCreator { get; set; }

        public string GroupName { get; set; }

        public ICollection<User> Users { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }

    public class Message
    {
        public int MessageId { get; set; }

        public string MessageText { get; set; }

        public Group Group { get; set; }

        public User User { get; set; }
    }
}