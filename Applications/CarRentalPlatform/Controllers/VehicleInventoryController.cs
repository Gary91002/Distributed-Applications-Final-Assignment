using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using CarRentalPlatform.Models;
using G2VehicleInventory.Domain.Enums;

namespace CarRentalPlatform.Controllers
{
	public class VehicleInventoryController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public VehicleInventoryController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		private HttpClient GetClient() => _httpClientFactory.CreateClient("ApiGateway");

		public async Task<IActionResult> Index()
		{
			var client = GetClient();
			var vehicles = await client.GetFromJsonAsync<List<VehicleViewModel>>("api/G2Vehicles/");
			return View(vehicles ?? new List<VehicleViewModel>());
		}

		public async Task<IActionResult> Details(int id)
		{
			var client = GetClient();
			var vehicle = await client.GetFromJsonAsync<VehicleViewModel>($"api/G2Vehicles/{id}");

			if (vehicle == null) return NotFound();

			return View(vehicle);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(VehicleViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var client = GetClient();
			var response = await client.PostAsJsonAsync("api/G2Vehicles/", model);

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index));
			}

			var errorMessage = await response.Content.ReadAsStringAsync();

			if (string.IsNullOrWhiteSpace(errorMessage))
			{
				errorMessage = "Unable to create vehicle.";
			}

			ModelState.AddModelError(string.Empty, errorMessage);
			return View(model);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var client = GetClient();
			var vehicle = await client.GetFromJsonAsync<VehicleViewModel>($"api/G2Vehicles/{id}");
			if (vehicle == null) return NotFound();
			return View(vehicle);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, string newStatus)
		{
			var client = GetClient();

			var existingVehicle = await client.GetFromJsonAsync<VehicleViewModel>($"api/G2Vehicles/{id}");
			if (existingVehicle == null)
			{
				return NotFound();
			}

			if (string.IsNullOrWhiteSpace(newStatus))
			{
				ModelState.AddModelError(string.Empty, "Please select a new status.");
				return View(existingVehicle);
			}

			if (!Enum.TryParse<VehicleStatus>(newStatus, out var parsedStatus))
			{
				ModelState.AddModelError(string.Empty, "Invalid status selected.");
				return View(existingVehicle);
			}

			var response = await client.PutAsJsonAsync($"api/G2Vehicles/{id}/status", parsedStatus);

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index));
			}

			var errorMessage = await response.Content.ReadAsStringAsync();

			if (string.IsNullOrWhiteSpace(errorMessage))
			{
				errorMessage = "Unable to update vehicle status.";
			}

			ModelState.AddModelError(string.Empty, errorMessage);

			return View(existingVehicle);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var client = GetClient();
			var vehicle = await client.GetFromJsonAsync<VehicleViewModel>($"api/G2Vehicles/{id}");
			if (vehicle == null) return NotFound();
			return View(vehicle);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var client = GetClient();
			await client.DeleteAsync($"api/G2Vehicles/{id}");
			return RedirectToAction(nameof(Index));
		}
	}
}