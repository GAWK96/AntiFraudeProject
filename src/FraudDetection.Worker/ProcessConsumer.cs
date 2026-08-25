using System;
using System.Diagnostics;
using FraudDetection.Application.DTOs;
using FraudDetection.Domain;
using FraudDetection.Domain.Entities;
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
			_logger.LogInformation("Mensagem recebida Id:{Id}", context.Message.Id);
			var getMessage = _context.MessageProcess.FirstOrDefault(x => x.MessageKey == context.Message.MessageKey);
			if(getMessage != null) 
			{
				_logger.LogInformation("Mensagem já processada Id:{Id}", context.Message.Id);
			}
			else 
			{ 
			_logger.LogInformation("Processando Transação Id:{Id}", context.Message.Id);
				var transaction = _context.Transactions.FirstOrDefault(x => x.Id == context.Message.Id);
				if (transaction != null)
				{
					try
					{
						transaction.Status = TransactionStatus.Processed;
						_context.MessageProcess.Add(new MessageProcess { ProcessedAt = DateTime.UtcNow, MessageKey = context.Message.MessageKey });
					}
					catch
					{
						_logger.LogError("Erro ao processar transação Id:{Id}", transaction.Id);
						throw;
					}
					await _context.SaveChangesAsync();
					_logger.LogInformation("Transação Processada Id:{Id}. Decisão:{Decision}", transaction.Id, transaction.Decision);
				}
			}
		}
	} 
}
