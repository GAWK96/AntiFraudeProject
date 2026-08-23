using FraudDetection.Application.DTOs;
using FraudDetection.Domain;
using FraudDetection.Domain.Entities;
using FraudDetection.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.Api.Controllers
{
	[ApiController]
	[Route("transactions")]
	public class TransactionController : ControllerBase
	{
		private readonly FraudDbContext _context;

		public TransactionController(FraudDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] TransactionRequestDto request, IBus bus) 
		{
			var transaction = new Transaction
			{
				CustomerId = request.CustomerId,
				Amount = request.Amount,
				CreatedAt = DateTime.UtcNow
			};
		   _context.Add(transaction);
			_context.SaveChanges();
			var getTransaction = _context.Transactions.FirstOrDefault(x => x.Id == transaction.Id);
			await bus.Publish(new TransactionResponseDto
			{
				Id = getTransaction.Id,
				CustomerId = getTransaction.CustomerId,
				Amount = getTransaction.Amount,
				CreatedAt = DateTime.UtcNow
			});
		   return Ok();
		}

		[HttpGet]
		public IActionResult Create(int id)
		{
			var transaction = _context.Transactions
								 .Where(x => x.Id == id)
								 .Select(x => new TransactionResponseDto
								 {
									 Id = x.Id,
									 CustomerId = x.CustomerId,
									 Amount = x.Amount,
									 Status = x.Status,
									 Decision = x.Decision,
									 CreatedAt = x.CreatedAt
								 });
			return Ok(transaction);
		}
	}
}
