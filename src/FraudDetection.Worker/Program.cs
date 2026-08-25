using FraudDetection.Infrastructure.Persistence;
using FraudDetection.Worker;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(resource =>
		resource.AddService("FraudDetection.Worker"))
	.WithTracing(tracing =>
	{
		tracing
		    .AddSource("MassTransit")
			.AddSqlClientInstrumentation()
			.AddConsoleExporter();
	});
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
			e.UseMessageRetry(r =>
					r.Interval(
						retryCount: 3,
						interval: TimeSpan.FromSeconds(2)));
			e.ConfigureConsumer<ProcessConsumer>(context);
		});
	});
});
var host = builder.Build();
host.Run();
