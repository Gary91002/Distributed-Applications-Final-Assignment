using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CarRentalPlatform.Models;

namespace CarRentalPlatform.Controllers
{
	public class CustomersController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public CustomersController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}


		private HttpClient GetClient() => _httpClientFactory.CreateClient("ApiGateway");

		// GET: Customers
		public async Task<IActionResult> Index()
		{
			var client = GetClient();
			var customers = await client.GetFromJsonAsync<List<G2Customer>>("api/G2CustomersApi/");
			return View(customers ?? new List<G2Customer>());
		}

		// GET: Customers/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var client = GetClient();
			var customer = await client.GetFromJsonAsync<G2Customer>($"api/G2CustomersApi/{id}");

			if (customer == null) return NotFound();

			return View(customer);
		}

		// GET: Customers/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Customers/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Phone,Email")] G2Customer g2Customer)
		{
			if (ModelState.IsValid)
			{
				var client = GetClient();
				var response = await client.PostAsJsonAsync("api/G2CustomersApi/", g2Customer);

				if (response.IsSuccessStatusCode)
					return RedirectToAction(nameof(Index));
			}
			return View(g2Customer);
		}

		// GET: Customers/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var client = GetClient();
			var customer = await client.GetFromJsonAsync<G2Customer>($"api/G2CustomersApi/{id}");

			if (customer == null) return NotFound();

			return View(customer);
		}

		// POST: Customers/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Phone,Email")] G2Customer g2Customer)
		{
			if (id != g2Customer.Id) return NotFound();

			if (ModelState.IsValid)
			{
				var client = GetClient();
				var response = await client.PutAsJsonAsync($"api/G2CustomersApi/{id}", g2Customer);

				if (response.IsSuccessStatusCode)
					return RedirectToAction(nameof(Index));
			}
			return View(g2Customer);
		}

		// GET: Customers/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var client = GetClient();
			var customer = await client.GetFromJsonAsync<G2Customer>($"api/G2CustomersApi/{id}");

			if (customer == null) return NotFound();

			return View(customer);
		}

		// POST: Customers/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var client = GetClient();
			await client.DeleteAsync($"api/G2CustomersApi/{id}");
			return RedirectToAction(nameof(Index));
		}
	}
}