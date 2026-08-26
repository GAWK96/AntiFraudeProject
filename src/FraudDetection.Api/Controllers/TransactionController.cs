using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Services;
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
		private readonly ILogger<TransactionController> _logger;
		private readonly IFraudDetectionService _service;

		public TransactionController(ILogger<TransactionController> logger, IFraudDetectionService service)
		{
			_logger = logger;
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] TransactionRequestDto request, IPublisher bus)
		{
			_logger.LogInformation("Criando transação Id:{Id}", request.IdempotencyKey);
			var checkTransaction = _service.GetTransactionByIdempotencyKey(request.IdempotencyKey);

			if (checkTransaction != null)
			{
				return Ok(checkTransaction);
			}
			else
			{
				{
					await _service.AddTransactionAndPublish(request);
					return CreatedAtAction(nameof(GetTransaction), checkTransaction);
				}
			}
		}

		[HttpGet]
		public IActionResult GetTransaction(int id)
		{
			var transaction = _service.GetTransactionById(id);
			return Ok(transaction);
		}
	}
}
