using FraudDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Infrastructure.Persistence
{
	public class FraudDbContext : DbContext
	{
		public FraudDbContext(DbContextOptions<FraudDbContext> options) : base(options)
		{

		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
						modelBuilder.Entity<Transaction>()
						   .HasIndex(x => x.IdempotencyKey)
						   .IsUnique();
		}
		public DbSet<Transaction> Transactions => Set<Transaction>();
	}
}
