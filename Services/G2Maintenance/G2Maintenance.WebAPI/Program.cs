using G2_Shared_Infrastructure;
using G2Maintenance.Application.Interfaces;
using G2Maintenance.Application.Services;
using G2Maintenance.Infrastructure.Data;
using G2Maintenance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var usageCounts = new Dictionary<string, int>();
builder.Services.AddSingleton(usageCounts);

var serviceName = "G2Maintenance";
var serviceVersion = "1.0.0";
builder.Services.AddHealthChecks();
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

builder.Services.AddDbContext<G2MaintenanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<G2IMaintenanceRepository, G2MaintenanceRepository>();
builder.Services.AddScoped<G2AddRepairHistory>();
builder.Services.AddScoped<G2GetRepairHistoryById>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<G2GlobalExceptionMiddleware>();
app.UseMiddleware<G2GatewayMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<G2MaintenanceDbContext>();

    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }
}


app.MapHealthChecks("/health").AllowAnonymous();
app.Run();