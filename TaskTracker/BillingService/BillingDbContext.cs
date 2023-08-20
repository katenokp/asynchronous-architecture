using Microsoft.EntityFrameworkCore;

namespace BillingService;

public class BillingDbContext: DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BillingCycle> BillingCycles { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<Error> Errors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(k => k.PublicId).HasConversion<string>();

        modelBuilder.Entity<BillingCycle>().Property(k => k.PublicId).HasConversion<string>();
        modelBuilder.Entity<BillingCycle>()
                    .HasMany(x => x.Transaction)
                    .WithOne(x => x.BillingCycle)
                    .HasForeignKey(x => x.BillingCycleId)
                    .HasPrincipalKey(x => x.Id);
        
        modelBuilder.Entity<Transaction>().Property(k => k.PublicId).HasConversion<string>();
        modelBuilder.Entity<Transaction>()
                    .HasOne(x => x.BillingCycle)
                    .WithMany(x => x.Transaction)
                    .HasForeignKey(x => x.BillingCycleId);
        
        modelBuilder.Entity<Account>().Property(k => k.PublicId).HasConversion<string>();

        modelBuilder.Entity<TaskEntity>().Property(k => k.PublicId).HasConversion<string>();
    }
}