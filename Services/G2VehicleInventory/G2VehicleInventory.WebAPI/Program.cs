using G2_Shared_Infrastructure;
using Microsoft.EntityFrameworkCore;
using G2VehicleInventory.Application.Interfaces;
using G2VehicleInventory.Application.Services;
using G2VehicleInventory.Infrastructure.Data;
using G2VehicleInventory.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<G2InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<G2IVehicleRepository, G2VehicleRepository>();
builder.Services.AddScoped<G2CreateVehicle>();
builder.Services.AddScoped<G2GetAllVehicles>();
builder.Services.AddScoped<G2GetVehicleById>();
builder.Services.AddScoped<G2UpdateVehicleStatus>();
builder.Services.AddScoped<G2DeleteVehicle>();
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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Apply migrations automatically in Docker / local SQL
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<G2InventoryDbContext>();

    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }
}

app.Run();

public partial class Program { }