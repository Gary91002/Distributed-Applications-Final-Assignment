using G2VehicleInventory.WebAPI.IntegrationTests.BaseClasses;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace G2VehicleInventory.WebAPI.IntegrationTests.Controllers
{
	public class G2VehiclesControllerIntegrationTests : IntegrationTestBase
	{
		public G2VehiclesControllerIntegrationTests(WebApplicationFactory<Program> factory)
			: base(factory)
		{
		}

		[Fact]
		public async Task GetAllVehicles_Should_ReturnOK()
		{
			var response = await Client.GetAsync("/api/G2Vehicles");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task GetVehicleById_Should_ReturnNotFound_WhenInvalidId()
		{
			var response = await Client.GetAsync("/api/G2Vehicles/99999");

			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task CreateVehicle_Should_ReturnOK()
		{
			var vehicleCode = GenerateVehicleCode("TEST");

			var newVehicle = new
			{
				vehicleCode = vehicleCode,
				locationId = 1,
				vehicleType = 0,
				vehicleStatus = 0
			};

			var response = await Client.PostAsJsonAsync("/api/G2Vehicles", newVehicle);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var vehicleId = await GetVehicleIdByCode(vehicleCode);

			Assert.True(vehicleId > 0);
		}

		[Fact]
		public async Task UpdateVehicleStatus_Should_ReturnOK()
		{
			var vehicleCode = GenerateVehicleCode("UPD");

			var newVehicle = new
			{
				vehicleCode = vehicleCode,
				locationId = 1,
				vehicleType = 0,
				vehicleStatus = 0
			};

			var createResponse = await Client.PostAsJsonAsync("/api/G2Vehicles", newVehicle);
			Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

			var id = await GetVehicleIdByCode(vehicleCode);

			var response = await Client.PutAsJsonAsync($"/api/G2Vehicles/{id}/status", 3);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task DeleteVehicle_Should_ReturnNoContent()
		{
			var vehicleCode = GenerateVehicleCode("DEL");

			var newVehicle = new
			{
				vehicleCode = vehicleCode,
				locationId = 1,
				vehicleType = 0,
				vehicleStatus = 0
			};

			var createResponse = await Client.PostAsJsonAsync("/api/G2Vehicles", newVehicle);
			Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

			var id = await GetVehicleIdByCode(vehicleCode);

			var response = await Client.DeleteAsync($"/api/G2Vehicles/{id}");

			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		private async Task<int> GetVehicleIdByCode(string vehicleCode)
		{
			var response = await Client.GetAsync("/api/G2Vehicles");
			response.EnsureSuccessStatusCode();

			var json = await response.Content.ReadFromJsonAsync<JsonElement>();

			if (json.ValueKind != JsonValueKind.Array)
				throw new Exception("Expected vehicle list response to be a JSON array.");

			foreach (var item in json.EnumerateArray())
			{
				string? code = null;

				if (item.TryGetProperty("vehicleCode", out var codeProp))
					code = codeProp.GetString();

				if (!string.Equals(code, vehicleCode, StringComparison.OrdinalIgnoreCase))
					continue;

				if (item.TryGetProperty("id", out var idProp))
					return idProp.GetInt32();

				if (item.TryGetProperty("vehicleId", out var vehicleIdProp))
					return vehicleIdProp.GetInt32();

				throw new Exception($"Vehicle '{vehicleCode}' was found, but no id field was present.");
			}

			throw new Exception($"Vehicle '{vehicleCode}' was not found after creation.");
		}

		private string GenerateVehicleCode(string prefix)
		{
			return $"{prefix}{Guid.NewGuid():N}".Substring(0, 9).ToUpper();
		}
	}
}