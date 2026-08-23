using System;
using System.Diagnostics;
using FraudDetection.Application.DTOs;
using FraudDetection.Domain;
using FraudDetection.Infrastructure.Persistence;
using MassTransit;

namespace FraudDetection.Worker
{
	internal sealed class ProcessConsumer : IConsumer<TransactionResponseDto>
	{
		private readonly ILogger<ProcessConsumer> _logger;
		private readonly FraudDbContext _context;
		public ProcessConsumer(ILogger<ProcessConsumer> logger, FraudDbContext context)
		{
			_logger = logger;
			_context = context;
		}
		public async Task Consume(ConsumeContext<TransactionResponseDto> context)
		{
			_logger.LogInformation("Processando Transação Id:{Id}", context.Message.Id);
			var transaction = _context.Transactions.FirstOrDefault(x => x.Id == context.Message.Id);
			if (transaction != null)
			{	transaction.Status = TransactionStatus.Processed;
				await _context.SaveChangesAsync();
				_logger.LogInformation("Transação Processada Id:{Id}", transaction.Id);
			}
		}
	} 
}
