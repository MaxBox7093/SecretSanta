using Microsoft.EntityFrameworkCore;
using SecretSanta.Models;

namespace SecretSanta.Contexts
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Participant> Participants { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Participants)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Recipient)
                .WithMany()
                .HasForeignKey(p => p.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
