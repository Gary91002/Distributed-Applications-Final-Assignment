using G2_Shared_Infrastructure;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var serviceName = "G2ApiGateway";
var serviceVersion = "1.0.0";

builder.Services.AddHealthChecks();
builder.Logging.ClearProviders();
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
	});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
	.ConfigureHttpClient((context, handler) =>
	{
		// This is to bypass the SSL error
		handler.SslOptions.RemoteCertificateValidationCallback =
			(sender, certificate, chain, sslPolicyErrors) => true;
	});

var app = builder.Build();
// Security Middleware
app.Use(async (context, next) =>
{
	if (context.Request.Path.StartsWithSegments("/health"))
	{
		await next();
		return;
	}
	const string API_KEY_HEADER = "X-GS-Api-Key";
	const string EXPECTED_API_KEY = "GS-Secret-Key-2111"; // MVC sends this

	if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedKey) ||
		extractedKey != EXPECTED_API_KEY)
	{
		context.Response.StatusCode = 401; // Unauthorized
		await context.Response.WriteAsync("Invalid or missing API Key.");
		return;
	}

	// Trusted header to block direct access
	context.Request.Headers.Append("X-From-Gateway", "GS-Gateway-Trusted-Token-111");

	await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapReverseProxy();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();
app.Run();
