namespace PsGenApi.DataModel;

public class PsGenDbContext(DbContextOptions<PsGenDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Account entity
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Accounts)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Token entity
        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Tokens");
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Tokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Secret).IsUnique();
        });
    }
}
