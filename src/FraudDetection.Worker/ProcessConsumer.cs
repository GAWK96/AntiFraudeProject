using System;
using System.Diagnostics;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;
using FraudDetection.Domain.Entities;
using FraudDetection.Infrastructure.Persistence;
using MassTransit;

namespace FraudDetection.Worker
{
	internal sealed class ProcessConsumer : IConsumer<TransactionResponseDto>
	{
		private readonly ILogger<ProcessConsumer> _logger;
		private readonly IFraudMetrics _metrics;
		private readonly IFraudDetectionService _service;
		public ProcessConsumer(ILogger<ProcessConsumer> logger, FraudDbContext context, IFraudMetrics metrics, IFraudDetectionService service)
		{
			_logger = logger;
			_metrics = metrics;
			_service = service;
		}
		public async Task Consume(ConsumeContext<TransactionResponseDto> context)
		{
			_logger.LogInformation("Mensagem recebida Id:{Id}", context.Message.Id);
			var getMessage = _service.GetMessageByKey(context.Message.MessageKey);
			if(getMessage != null) 
			{
				_logger.LogInformation("Mensagem já processada Id:{Id}", context.Message.Id);
				_metrics.DuplicatedMessage();
			}
			else 
			{ 
				var transaction = _service.GetTransactionById(context.Message.Id);
				if (transaction != null)
				{
						var decision = await _service.ProcessTransaction(transaction,context.Message.MessageKey);
						_metrics.TransactionProcessed(decision);
						_logger.LogInformation(
							"Transação processada Id:{Id}. Decisão:{Decision}",
							transaction.Id,
							transaction.Decision);
				}
			}
		}
	} 
}
