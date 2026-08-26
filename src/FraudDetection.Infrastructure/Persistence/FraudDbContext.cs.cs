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
						modelBuilder.Entity<TransactionModel>()
						   .ToTable("Transaction")
						   .HasIndex(x => x.IdempotencyKey)
						   .IsUnique();
			modelBuilder.Entity<MessageModel>()
			   .ToTable("Message")
			   .HasIndex(x => x.MessageKey)
			   .IsUnique();
		}
		public DbSet<TransactionModel> Transactions => Set<TransactionModel>();

		public DbSet<MessageModel> MessageModel=> Set<MessageModel>();
	}
}
