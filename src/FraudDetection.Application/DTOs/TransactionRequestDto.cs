using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Domain;

namespace FraudDetection.Application.DTOs
{
	public class TransactionRequestDto
	{
		public Guid IdempotencyKey { get; set; }

		public TransactionStatus Status { get; set; }
		public required string CustomerId { get; set; }
		public decimal Amount { get; set; }
	}
}
