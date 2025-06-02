namespace PsGenApi.DataModel;

public class PsGenDbContext(DbContextOptions<PsGenDbContext> options) : DbContext(options)
{
	public DbSet<DbUser> Users { get; set; } = null!;

	public DbSet<DbAccount> Accounts { get; set; } = null!;

	public DbSet<DbToken> Tokens { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Configure User entity
		modelBuilder.Entity<DbUser>(entity =>
		{
			entity.ToTable("Users");
			entity.HasIndex(e => e.Username).IsUnique();
			entity.HasIndex(e => e.Email).IsUnique();
		});

		// Configure Account entity
		modelBuilder.Entity<DbAccount>(entity =>
		{
			entity.ToTable("Accounts");
			entity.HasOne(e => e.User)
				.WithMany(u => u.Accounts)
				.HasForeignKey(e => e.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// Configure Token entity
		modelBuilder.Entity<DbToken>(entity =>
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