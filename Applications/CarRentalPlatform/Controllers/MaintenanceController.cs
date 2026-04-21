using CarRentalPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalPlatform.Controllers
{
	public class MaintenanceController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public MaintenanceController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		public IActionResult History()
		{
			return View(new List<RepairHistoryViewModel>());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> History(int vehicleId)
		{
			var client = _httpClientFactory.CreateClient("MaintenanceApi");
			var repairs = await client.GetFromJsonAsync<List<RepairHistoryViewModel>>(
				$"api/maintenance/vehicles/{vehicleId}/repairs");

			ViewBag.VehicleId = vehicleId;
			return View(repairs ?? new List<RepairHistoryViewModel>());
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View(new RepairHistoryViewModel
			{
				RepairDate = DateTime.Today
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(RepairHistoryViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var client = _httpClientFactory.CreateClient("MaintenanceApi");
			var response = await client.PostAsJsonAsync("api/maintenance/", model);

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(History), new { vehicleId = model.VehicleId });
			}

			ModelState.AddModelError(string.Empty, "Unable to create maintenance record.");
			return View(model);
		}

		public async Task<IActionResult> Usage()
		{
			var client = _httpClientFactory.CreateClient("MaintenanceApi");
			var result = await client.GetFromJsonAsync<object>("api/maintenance/usage");
			return View(result);
		}
	}
}