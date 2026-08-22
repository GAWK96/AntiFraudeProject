using FraudDetection.Application.DTOs;
using FraudDetection.Domain.Entities;
using FraudDetection.Infrastructure.Persistence;
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
		public IActionResult Create([FromBody] TransactionRequestDto request) 
		{
		   _context.Add(new Transaction
		   {
			   CustomerId = request.CustomerId,
			   Amount = request.Amount,
			   CreatedAt = DateTime.UtcNow
		   });
		   _context.SaveChanges();
		   return Ok();
		}
	}
}
