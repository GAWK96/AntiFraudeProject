using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FraudDetection.Infrastructure
{
	public static class RabbitMqPublisher
	{
		public static void AddRabbitMQService(this IServiceCollection services)
		{
			services.AddMassTransit(busConfigurator =>
			{
				busConfigurator.UsingRabbitMq((ctx, cfg) =>
			 {
				   cfg.Host(("amqp://localhost:5672"), host =>
				   {
					   host.Username("guest");
					   host.Password("guest");
				   });

				 cfg.ConfigureEndpoints(ctx);
			   });
			});
		}
	}
}