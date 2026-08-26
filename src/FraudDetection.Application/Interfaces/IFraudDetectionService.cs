using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Application.DTOs;
using FraudDetection.Domain;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Interfaces
{
	public interface IFraudDetectionService
	{
		public TransactionResponseDto? GetTransactionById(int id);
		public MessageProcessDto? GetMessageByKey(Guid messageKey);
		Task<TransactionResponseDto> AddTransactionAndPublish(TransactionRequestDto transaction);
		Task<MessageProcessDto> AddMessage(MessageProcessDto message);
		public TransactionResponseDto? GetTransactionByIdempotencyKey(Guid idempotencyKey);
		public Task<TransactionDecision> SetTransactionDecisionAndStatus(TransactionResponseDto transaction);

		Task<TransactionDecision> ProcessTransaction(TransactionResponseDto transaction, Guid messageKey);
	}
}
