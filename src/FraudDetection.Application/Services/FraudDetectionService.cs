using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;
using FraudDetection.Domain.Entities;
using MassTransit;
using MassTransit.Transports.Fabric;
using Microsoft.Extensions.Logging;
using TransactionStatus = FraudDetection.Domain.TransactionStatus;

namespace FraudDetection.Application.Services
{
	public class FraudDetectionService : IFraudDetectionService
	{
		private readonly IFraudDetectionRepository _repository;
		private readonly IPublisher _bus;
		private readonly ILogger<IFraudDetectionService> _logger;

		private readonly IFraudMetrics _metrics;

		private readonly IUnitOfWork _unitOfWork;
		public FraudDetectionService(IFraudDetectionRepository repository, IPublisher bus, ILogger<IFraudDetectionService> logger, IUnitOfWork unitOfWork)
		{
			_repository = repository;
			_bus = bus;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public TransactionResponseDto? GetTransactionById(int id)
		{
			var getTransaction = _repository.GetTransactionById(id);
			if (getTransaction == null)
			{
				return null;
			}	
			var transactionMapped = new TransactionResponseDto
			{
				Id = getTransaction.Id,
				CustomerId = getTransaction.CustomerId,
				Amount = getTransaction.Amount,
				Status = getTransaction.Status,
				Decision = getTransaction.Decision,
				CreatedAt = getTransaction.CreatedAt,
				MessageKey = Guid.NewGuid(),
			};
			return transactionMapped;
		}

		public TransactionResponseDto? GetTransactionByIdempotencyKey(Guid idempotencyKey)
		{
			var getTransaction = _repository.GetTransactionByIdempotencyKey(idempotencyKey);
			if (getTransaction == null)
			{
				return null;
			}
			var transactionMapped = new TransactionResponseDto
			{
				Id = getTransaction.Id,
				CustomerId = getTransaction.CustomerId,
				Amount = getTransaction.Amount,
				Status = getTransaction.Status,
				Decision = getTransaction.Decision,
				CreatedAt = getTransaction.CreatedAt,
				MessageKey = Guid.NewGuid(),
			};
			return transactionMapped;
		}

		public MessageProcessDto? GetMessageByKey(Guid messageKey)
		{
			var message = _repository.GetMessageByMessageKey(messageKey);
			if (message == null)
			{
				return null;
			}
			var messageMapped = new MessageProcessDto
			{
				Id = message.Id,
				TransactionId = message.TransactionId,
				ProcessedAt = message.ProcessedAt,
				MessageKey = message.MessageKey,
			};
			return messageMapped;
		}

		public async Task<TransactionResponseDto> AddTransactionAndPublish(TransactionRequestDto transaction)
		{
			var transactionMapped = new TransactionModel
			{
				CustomerId = transaction.CustomerId,
				Amount = transaction.Amount,
				CreatedAt = DateTime.UtcNow,
				IdempotencyKey = transaction.IdempotencyKey,
				Status = TransactionStatus.Pending
			};
			var transactionAdded = await _repository.AddTransaction(transactionMapped);
			await _unitOfWork.SaveChangesAsync();
			_logger.LogInformation("Transação criada Id:{Id}", transactionAdded.IdempotencyKey);
			var getTransaction = _repository.GetTransactionByIdempotencyKey(transactionAdded.IdempotencyKey);
			var message = new TransactionResponseDto
			{
				Id = getTransaction.Id,
				CustomerId = transactionAdded.CustomerId,
				Amount = transactionAdded.Amount,
				CreatedAt = DateTime.UtcNow,
				MessageKey = Guid.NewGuid(),
			};
			await _unitOfWork.SaveChangesAsync();
			await _bus.PublishAsync(message);
			_logger.LogInformation("Mensagem publicada");
			return message;
		}

		public async Task<MessageProcessDto> AddMessage(MessageProcessDto message)
		{
			var messageMapped = new MessageModel
			{
				TransactionId = message.TransactionId,
				ProcessedAt = DateTime.UtcNow,
				MessageKey = message.MessageKey
			};
			await _repository.AddMessage(messageMapped);
			return message;
		}

		public async Task<TransactionDecision> SetTransactionDecisionAndStatus(TransactionResponseDto transaction)
		{
		var getTransaction = _repository.GetTransactionById(transaction.Id);
			getTransaction.Status = TransactionStatus.Processing;
			await _unitOfWork.SaveChangesAsync();
			var transactionProcessed = _repository.SetTransactionDecisionAndStatus(getTransaction);
			return transactionProcessed;
		}

		public async Task<TransactionDecision> ProcessTransaction(TransactionResponseDto transaction,Guid messageKey)
		{
			try
			{
				_logger.LogInformation("Transação sendo processada Id:{Id}", transaction.Id);
				var decision = await SetTransactionDecisionAndStatus(transaction);

				var message = new MessageModel
				{
					TransactionId = transaction.Id,
					MessageKey = messageKey,
					ProcessedAt = DateTime.UtcNow
				};

				await _repository.AddMessage(message);

				await _unitOfWork.SaveChangesAsync();

				return decision;
			}
			catch (Exception ex)
			{
				_metrics.ProcessingError();

				_logger.LogError(
					ex,
					"Erro ao processar transação Id:{Id}",
					transaction.Id);

				throw;
			}
		}

	}
}