using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Application.DTOs;
using FraudDetection.Domain;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Interfaces
{
	public interface IFraudDetectionRepository
	{
		public TransactionModel GetTransactionById(int id);
		public TransactionModel? GetTransactionByIdempotencyKey(Guid idempotencyKey);
		public MessageModel GetMessageByMessageKey(Guid messageKey);
		public Task<MessageModel> AddMessage(MessageModel message);

		public Task<TransactionModel> AddTransaction(TransactionModel transaction);

		public TransactionDecision SetTransactionDecisionAndStatus(TransactionModel transaction);
	}
}
