namespace FraudDetection.Domain.Entities;

public class Transaction
{
public int Id { get; set; }

public string CustomerId { get; set; }
public decimal Amount { get; set; }

public TransactionStatus Status { get; set; }

public TransactionDecision Decision { get; set; }

public Guid IdempotencyKey { get; set; }
public DateTime CreatedAt { get; set; }
}
