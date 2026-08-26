using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Domain;

namespace FraudDetection.Application.DTOs
{
	public class TransactionResponseDto
	{
		public int Id { get; set; }
		public required string CustomerId { get; set; }
		public decimal Amount { get; set; }
		public TransactionStatus Status { get; set; }
		public TransactionDecision? Decision { get; set; }
		public DateTime CreatedAt { get; set; }

		public Guid MessageKey { get; set; }
	}
}
