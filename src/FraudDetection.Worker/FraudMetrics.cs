using System.Diagnostics.Metrics;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;

public class FraudMetrics : IFraudMetrics
{
	private readonly Counter<long> _transactionsProcessed;
	private readonly Counter<long> _processingErrors;
	private readonly Counter<long> _duplicatedMessages;

	public FraudMetrics(IMeterFactory meterFactory)
	{
		var meter = meterFactory.Create("FraudDetection.Worker");

		_transactionsProcessed = meter.CreateCounter<long>(
			"transactions_processed_total");

		_processingErrors = meter.CreateCounter<long>(
			"transaction_processing_errors_total");

		_duplicatedMessages = meter.CreateCounter<long>(
			"duplicated_messages_total");
	}

	public void TransactionProcessed(TransactionDecision decision)
	{
		_transactionsProcessed.Add(
			1,
			new KeyValuePair<string, object?>(
				"decision",
				decision.ToString()));
	}

	public void ProcessingError()
	{
		_processingErrors.Add(1);
	}

	public void DuplicatedMessage()
	{
		_duplicatedMessages.Add(1);
	}
}