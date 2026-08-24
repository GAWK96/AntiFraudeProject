using System;
using System.Collections.Generic;
using System.Text;

namespace FraudDetection.Domain.Entities
{
	public class MessageProcess
	{
		public int Id { get; set; }
		public int TransactionId { get; set; }
		public DateTime ProcessedAt { get; set; }
	}
}
