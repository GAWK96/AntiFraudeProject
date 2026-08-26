using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Application.Interfaces;
using FraudDetection.Infrastructure.Persistence;

namespace FraudDetection.Infrastructure
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly FraudDbContext _context;

		public UnitOfWork(FraudDbContext context)
		{
			_context = context;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
