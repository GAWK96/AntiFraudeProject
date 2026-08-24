using System;
using System.Collections.Generic;
using FraudDetection.Application.Interfaces;
using MassTransit;

namespace FraudDetection.Infrastructure
{
	public class Publisher : IPublisher
	{
		private readonly IPublishEndpoint _publishEndpoint;

		public Publisher(IPublishEndpoint publishEndpoint)
		{
			_publishEndpoint = publishEndpoint;
		}
		public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
		{
			return _publishEndpoint.Publish(message, cancellationToken);
		}
	}
}
