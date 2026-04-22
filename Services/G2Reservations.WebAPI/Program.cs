using G2Reservations.WebAPI.Models;
using G2_Shared_Infrastructure;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serviceName = "G2Reservations";
var serviceVersion = "1.0.0";

/*builder.Logging.ClearProviders();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Logging.AddConsole();
builder.Logging.AddOpenTelemetry(options =>
{
	options.SetResourceBuilder(
		ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion));
	options.IncludeFormattedMessage = true;
	options.IncludeScopes = true;
	options.ParseStateValues = true;
	options.AddConsoleExporter();
});

builder.Services.AddOpenTelemetry()
	.ConfigureResource(resource => resource.AddService(serviceName: serviceName, serviceVersion: serviceVersion))
	.WithTracing(tracing =>
	{
		tracing
			.AddAspNetCoreInstrumentation(options =>
			{
				options.RecordException = true;
				options.Filter = httpContext =>
					!httpContext.Request.Path.StartsWithSegments("/swagger") &&
					!httpContext.Request.Path.StartsWithSegments("/favicon.ico");
			})
			.AddHttpClientInstrumentation()
			.AddConsoleExporter();
	})
	.WithMetrics(metrics =>
	{
		metrics
			.AddAspNetCoreInstrumentation()
			.AddHttpClientInstrumentation()
			.AddConsoleExporter();
	});*/

builder.Services.AddDbContext<G2ReservationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("CustomersApi", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CustomersApi"]!);
	client.DefaultRequestHeaders.Add("X-From-Gateway", "GS-Gateway-Trusted-Token-111");
});

builder.Services.AddHttpClient("VehicleInventoryApi", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:VehicleInventoryApi"]!);
	client.DefaultRequestHeaders.Add("X-From-Gateway", "GS-Gateway-Trusted-Token-111");
});

var app = builder.Build();

app.UseMiddleware<G2GlobalExceptionMiddleware>();
app.UseMiddleware<G2GatewayMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<G2ReservationDbContext>();

	if (db.Database.IsRelational())
	{
		db.Database.Migrate();
	}
	else
	{
		db.Database.EnsureCreated();
	}
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();
app.Run();