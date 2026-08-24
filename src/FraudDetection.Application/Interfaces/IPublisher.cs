using System;
using System.Collections.Generic;
using System.Text;
using MassTransit;

namespace FraudDetection.Application.Interfaces
{
	public interface IPublisher
	{
		Task PublishAsync<T>(T message, CancellationToken cancellationToken = default(CancellationToken)) where T : class;

	}
}
