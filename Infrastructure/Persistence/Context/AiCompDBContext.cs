using AiComp.Application;
using AiComp.Application.DTOs.ValueObjects;
using AiComp.Core.Entities;
using AiComp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiComp.Infrastructure.Persistence.Context
{
    public class AiCompDBContext : DbContext
    {
        public AiCompDBContext(DbContextOptions<AiCompDBContext> option) :  base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasOne(x => x.Conversation).WithOne(p => p.User);
            modelBuilder.Entity<User>().HasOne(x => x.Profile).WithOne(p => p.User);
            modelBuilder.Entity<User>().HasMany(x => x.MoodLogs).WithOne(p => p.User).HasForeignKey(k => k.UserId);
            modelBuilder.Entity<User>().HasMany(x => x.MoodMessages).WithOne(p => p.User).HasForeignKey(p => p.UserId);
            modelBuilder.Entity<User>().HasMany(x => x.Journals).WithOne(p => p.User).HasForeignKey(p => p.UserId);

            var admin = new User("admin@aicomp.com");
            admin.AddPassword("string");
            modelBuilder.Entity<User>().HasData(admin);
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(u => u.UserId).IsRequired();

                //this is to save the List of chat converse as json and be retrived and converted to list of object
                entity.Property(c => c.Conversations)
                .HasConversion(new JsonValueConverter<List<ChatConverse>>())
                .HasColumnType("json")
                .Metadata.SetValueComparer(new JsonValueComparer<List<ChatConverse>>());

                //The value  comparer is to determine if two instances of a collection or complex type are equal.
                //This is essential for change tracking, where EF Core tracks modifications to entity properties.
                //By serializing the collections to JSON strings, it ensures a deep comparison,
                //meaning it compares the entire structure and content of the collections, not just references or shallow properties
            });

        }


        public DbSet<User> Users { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<MoodLog> MoodLogs { get; set; }
        public DbSet<MoodMessage> MoodMesages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Profile> Profiles { get; set; }
    }
}
