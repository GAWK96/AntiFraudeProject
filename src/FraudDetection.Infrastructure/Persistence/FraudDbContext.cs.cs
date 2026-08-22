using FraudDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Infrastructure.Persistence
{
	public class FraudDbContext : DbContext
	{
	    public FraudDbContext(DbContextOptions<FraudDbContext> options) : base(options)
		{
		  
		}

		public DbSet<Transaction> Transactions => Set<Transaction>();
	}
}
