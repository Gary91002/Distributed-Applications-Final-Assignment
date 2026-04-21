using System.Net.Http.Json;
using System.Text.Json;
using CarRentalPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentalPlatform.Controllers
{
	public class G2ReservationsController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public G2ReservationsController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		private HttpClient GetClient() => _httpClientFactory.CreateClient("ApiGateway");

		[HttpGet]
		public async Task<IActionResult> Index(int? customerId)
		{
			var model = new G2ReservationsPageViewModel();

			await LoadDropdowns(model);

			if (customerId.HasValue && customerId.Value > 0)
			{
				model.SelectedCustomerId = customerId.Value;
				model.Reservation.CustomerId = customerId.Value;
				model.ReservationHistory = await GetReservationHistory(customerId.Value);
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Reserve(G2ReservationsPageViewModel model)
		{
			await LoadDropdowns(model);

			if (!ModelState.IsValid)
			{
				if (model.Reservation.CustomerId > 0)
				{
					model.SelectedCustomerId = model.Reservation.CustomerId;
					model.ReservationHistory = await GetReservationHistory(model.Reservation.CustomerId);
				}

				return View("Index", model);
			}

			var client = GetClient();

			var response = await client.PostAsJsonAsync("api/G2Reservations/", model.Reservation);

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index), new { customerId = model.Reservation.CustomerId });
			}

			var errorMessage = await GetErrorMessage(response, "Unable to create reservation.");
			ModelState.AddModelError(string.Empty, errorMessage);

			model.SelectedCustomerId = model.Reservation.CustomerId;
			model.ReservationHistory = await GetReservationHistory(model.Reservation.CustomerId);

			return View("Index", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SearchHistory(G2ReservationsPageViewModel model)
		{
			await LoadDropdowns(model);

			if (model.Reservation.CustomerId <= 0)
			{
				ModelState.AddModelError(string.Empty, "Please select a customer.");
				return View("Index", model);
			}

			model.SelectedCustomerId = model.Reservation.CustomerId;
			model.ReservationHistory = await GetReservationHistory(model.Reservation.CustomerId);

			return View("Index", model);
		}

		private async Task LoadDropdowns(G2ReservationsPageViewModel model)
		{
			var client = GetClient();

			var customers = await client.GetFromJsonAsync<List<G2Customer>>("api/G2CustomersApi/") ?? new List<G2Customer>();
			var vehicles = await client.GetFromJsonAsync<List<VehicleViewModel>>("api/G2Vehicles/") ?? new List<VehicleViewModel>();

			model.Customers = customers
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = $"{c.FirstName} {c.LastName} (ID: {c.Id})"
				})
				.ToList();

			model.Vehicles = vehicles
				.Select(v => new SelectListItem
				{
					Value = v.Id.ToString(),
					Text = $"{v.VehicleCode} - {v.VehicleType} - {v.VehicleStatus}"
				})
				.ToList();
		}

		private async Task<List<G2ReservationHistoryViewModel>> GetReservationHistory(int customerId)
		{
			var client = GetClient();

			var history = await client.GetFromJsonAsync<List<G2ReservationHistoryViewModel>>(
				$"api/G2Reservations/customer/{customerId}/history");

			return history ?? new List<G2ReservationHistoryViewModel>();
		}

		private async Task<string> GetErrorMessage(HttpResponseMessage response, string fallback)
		{
			var content = await response.Content.ReadAsStringAsync();

			if (string.IsNullOrWhiteSpace(content))
				return fallback;

			try
			{
				using var doc = JsonDocument.Parse(content);

				if (doc.RootElement.TryGetProperty("message", out var messageProp))
					return messageProp.GetString() ?? fallback;

				if (doc.RootElement.TryGetProperty("error", out var errorProp))
					return errorProp.GetString() ?? fallback;
			}
			catch
			{
			}

			return fallback;
		}
	}
}