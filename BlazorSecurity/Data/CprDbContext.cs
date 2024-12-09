using Microsoft.EntityFrameworkCore;

namespace BlazorSecurity.Data;

public class CprDbContext(DbContextOptions<CprDbContext> options) : DbContext(options)
{
    public DbSet<CprUser> CprUsers { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDo>()
            .HasOne<CprUser>() // Assuming ApplicationUser is your user class
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Foreign key relationship

        modelBuilder.Entity<ToDo>()
            .HasIndex(t => new { t.UserId, t.ToDoItem }) // Ensure uniqueness per user
            .IsUnique();
    }
}


