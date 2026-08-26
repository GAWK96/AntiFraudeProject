using System;
using System.Collections.Generic;
using System.Text;
using FraudDetection.Domain;

namespace FraudDetection.Application.Interfaces
{
	public interface IFraudMetrics
	{
		void TransactionProcessed(TransactionDecision decision);
		void ProcessingError();
		void DuplicatedMessage();
	}
}
