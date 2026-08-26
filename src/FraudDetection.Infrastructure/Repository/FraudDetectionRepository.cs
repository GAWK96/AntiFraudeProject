using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;
using FraudDetection.Domain.Entities;
using FraudDetection.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Infrastructure.Repository
{
	public class FraudDetectionRepository : IFraudDetectionRepository
	{
		private readonly FraudDbContext _context;

		public FraudDetectionRepository(FraudDbContext context)
		{
			_context = context;
		}

		public TransactionModel? GetTransactionById(int id)
		{
			var transaction = _context.Transactions.FirstOrDefault(x => x.Id == id);
			return transaction;
		}

		public TransactionModel? GetTransactionByIdempotencyKey(Guid idempotencyKey)
		{
			var transaction = _context.Transactions.FirstOrDefault(x => x.IdempotencyKey == idempotencyKey);
			return transaction;
		}

		public MessageModel GetMessageByMessageKey(Guid messageKey)
		{
			var message = _context.MessageModel.FirstOrDefault(x => x.MessageKey == messageKey);
			return message;
		}

		public async Task<MessageModel> AddMessage(MessageModel message)
		{
			_context.MessageModel.Add(message);
			return message;
		}

		public async Task<TransactionModel> AddTransaction(TransactionModel transaction)
		{
			_context.Transactions.Add(transaction);
			return transaction;
		}

		public TransactionDecision SetTransactionDecisionAndStatus(TransactionModel transaction)
		{
		var getTransaction = GetTransactionById(transaction.Id);
			TransactionDecision decision;
			if (getTransaction.Amount <= 1000)
			{
				decision = TransactionDecision.Approved;
			}
			else if (getTransaction.Amount > 1000 && getTransaction.Amount <= 5000)
			{
				decision = TransactionDecision.Review;
			}
			else 
			{
				decision = TransactionDecision.Rejected;
			}
			getTransaction.Status = TransactionStatus.Processed;
			getTransaction.Decision = decision;
			return decision;
		}
	}
}
