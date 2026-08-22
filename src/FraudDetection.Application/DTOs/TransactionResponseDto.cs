using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Domain;

namespace FraudDetection.Application.DTOs
{
	internal class TransactionResponseDto
	{
		public Guid Id { get; set; }
		public string CustomerId { get; set; }
		public decimal Amount { get; set; }
		public TransactionStatus Status { get; set; }
		public TransactionDecision? Decision { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
