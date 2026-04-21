using CarRentalPlatform.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
// 1. Get values from appsettings.json
var gatewayUrl = builder.Configuration["ApiGateway:BaseUrl"]!;
const string gatewayKey = "GS-Secret-Key-2111"; // Must match the Gateway's expected key

// 2. Register the "ApiGateway" client used by your Controllers
builder.Services.AddHttpClient("ApiGateway", client =>
{
	client.BaseAddress = new Uri(gatewayUrl);
	client.DefaultRequestHeaders.Add("X-GS-Api-Key", gatewayKey);
});

// 3. Register the specific "MaintenanceApi" client (because it needs the EXTRA internal key)
builder.Services.AddHttpClient("MaintenanceApi", client =>
{
	client.BaseAddress = new Uri(gatewayUrl);
	client.DefaultRequestHeaders.Add("X-GS-Api-Key", gatewayKey);
	client.DefaultRequestHeaders.Add("X-Api-Key", "MY_SECRET_KEY_123"); // Maintenance internal logic
});

builder.Services.AddControllersWithViews();

// Database context for local storage if needed (Optional if everything is moved to APIs)
builder.Services.AddDbContext<CustomerProfileContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHealthChecks("/health").AllowAnonymous();
app.Run();