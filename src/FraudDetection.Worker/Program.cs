using FraudDetection.Infrastructure.Persistence;
using FraudDetection.Worker;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<FraudDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<ProcessConsumer>();

	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host("amqp://localhost:5672", h =>
		{
			h.Username("guest");
			h.Password("guest");
		});

		cfg.ReceiveEndpoint("process-transaction", e =>
		{
			e.ConfigureConsumer<ProcessConsumer>(context);
		});
	});
});
var host = builder.Build();
host.Run();
