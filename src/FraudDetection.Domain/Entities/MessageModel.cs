using System;
using System.Collections.Generic;
using System.Text;

namespace FraudDetection.Domain.Entities
{
	public class MessageModel
	{
		public int Id { get; set; }
		public int TransactionId { get; set; }
		public DateTime ProcessedAt { get; set; }
		public Guid MessageKey { get; set; }
	}
}
