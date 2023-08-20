using Microsoft.EntityFrameworkCore;

namespace TaskManagement;

public class TasksDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(k => k.PublicId).HasConversion<string>();
        modelBuilder.Entity<TaskEntity>().Property(k => k.PublicId).HasConversion<string>();
    }
}