using G2VehicleInventory.Infrastructure;
using G2VehicleInventory.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace G2VehicleInventory.WebAPI.IntegrationTests.BaseClasses
{
	public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
	{
		protected readonly HttpClient Client;

		public IntegrationTestBase(WebApplicationFactory<Program> factory)
		{
			var customFactory = factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					// Remove SQL Server DB
					var descriptor = services.SingleOrDefault(
						d => d.ServiceType == typeof(DbContextOptions<G2InventoryDbContext>));

					if (descriptor != null)
						services.Remove(descriptor);

					// Add InMemory DB
					services.AddDbContext<G2InventoryDbContext>(options =>
					{
						options.UseInMemoryDatabase("TestDb");
					});
				});
			});

			Client = customFactory.CreateClient();

			// bypass gateway middleware
			Client.DefaultRequestHeaders.Add("X-From-Gateway", "GS-Gateway-Trusted-Token-111");
		}
	}
}