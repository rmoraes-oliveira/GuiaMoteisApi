using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Suite> Suites { get; set; }
    public DbSet<Motel> Motels { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<TypeSuite> TypeSuites { get; set; }
    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Motel>()
            .HasMany(m => m.Suites)
            .WithOne(s => s.Motel)
            .HasForeignKey(s => s.MotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Suite>()
            .HasOne(s => s.Motel)
            .WithMany(m => m.Suites)
            .HasForeignKey(s => s.MotelId);

        modelBuilder.Entity<Suite>()
            .HasOne(s => s.TypeSuite)
            .WithMany(t => t.Suites)
            .HasForeignKey(s => s.TypeSuiteId);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Suite)
            .WithMany()
            .HasForeignKey(r => r.SuiteId);
    }
}
