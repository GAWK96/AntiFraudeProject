using System;
using System.Collections.Generic;
using System.Text;

namespace FraudDetection.Application.DTOs
{
	public class TransactionRequestDto
	{
		public int Id { get; set; }
		public string CustomerId { get; set; }
		public decimal Amount { get; set; }
	}
}
