using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Services;
using FraudDetection.Infrastructure;
using FraudDetection.Infrastructure.Persistence;
using FraudDetection.Infrastructure.Repository;
using FraudDetection.Worker;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFraudMetrics, FraudMetrics>();
builder.Services.AddScoped<IFraudDetectionRepository, FraudDetectionRepository>();
builder.Services.AddScoped<IFraudDetectionService, FraudDetectionService>();
builder.Services.AddScoped<IPublisher, Publisher>();
builder.Services.AddHostedService<Worker>();
builder.Services
	.AddOpenTelemetry()
	.WithMetrics(metrics =>
	{
		metrics
			.AddMeter("FraudDetection.Worker")
			.AddConsoleExporter();
	})
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
		var host = builder.Configuration["RabbitMq:Host"];
		var username = builder.Configuration["RabbitMq:Username"];
		var password = builder.Configuration["RabbitMq:Password"];

		cfg.Host(host, "/", h =>
		{
			h.Username(username!);
			h.Password(password!);
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
