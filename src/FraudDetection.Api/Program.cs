using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Services;
using FraudDetection.Infrastructure;
using FraudDetection.Infrastructure.Persistence;
using FraudDetection.Infrastructure.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPublisher, Publisher>();
builder.Services.AddScoped<IFraudDetectionService, FraudDetectionService>();
builder.Services.AddScoped<IFraudDetectionRepository, FraudDetectionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<FraudDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(resource =>
		resource.AddService("FraudDetection.Api"))
	.WithTracing(tracing =>
	{
		tracing
			.AddSource("MassTransit")
			.AddAspNetCoreInstrumentation()
			.AddHttpClientInstrumentation()
			.AddSqlClientInstrumentation()
			.AddConsoleExporter();
	});
builder.Services.AddMassTransit(x =>
{
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
	});
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<FraudDbContext>();
	context.Database.Migrate();
}
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();