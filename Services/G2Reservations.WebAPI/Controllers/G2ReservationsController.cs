using G2Reservations.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G2Reservations.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class G2ReservationsController : ControllerBase
	{
		private readonly G2ReservationDbContext _context;

		public G2ReservationsController(G2ReservationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<G2ReservationDto>>> GetReservations()
		{
			var reservations = await _context.Reservations
				.OrderByDescending(r => r.CreatedDate)
				.Select(r => new G2ReservationDto
				{
					Id = r.Id,
					CustomerId = r.CustomerId,
					VehicleId = r.VehicleId,
					CreatedDate = r.CreatedDate,
					StartDate = r.StartDate,
					EndDate = r.EndDate
				})
				.ToListAsync();

			return Ok(reservations);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<G2ReservationDto>> GetReservation(int id)
		{
			var reservation = await _context.Reservations.FindAsync(id);

			if (reservation == null)
			{
				return NotFound(new
				{
					error = "NotFound",
					message = "Reservation not found."
				});
			}

			return Ok(new G2ReservationDto
			{
				Id = reservation.Id,
				CustomerId = reservation.CustomerId,
				VehicleId = reservation.VehicleId,
				CreatedDate = reservation.CreatedDate,
				StartDate = reservation.StartDate,
				EndDate = reservation.EndDate
			});
		}

		[HttpGet("customer/{customerId}")]
		public async Task<ActionResult<IEnumerable<G2ReservationDto>>> GetReservationsByCustomer(int customerId)
		{
			var reservations = await _context.Reservations
				.Where(r => r.CustomerId == customerId)
				.OrderByDescending(r => r.CreatedDate)
				.Select(r => new G2ReservationDto
				{
					Id = r.Id,
					CustomerId = r.CustomerId,
					VehicleId = r.VehicleId,
					CreatedDate = r.CreatedDate,
					StartDate = r.StartDate,
					EndDate = r.EndDate
				})
				.ToListAsync();

			return Ok(reservations);
		}

		[HttpPost]
		public async Task<ActionResult<G2ReservationDto>> PostReservation(G2CreateReservationDto dto)
		{
			if (dto.CustomerId <= 0)
			{
				return BadRequest(new
				{
					error = "InvalidParameter",
					message = "CustomerId must be greater than zero."
				});
			}

			if (dto.VehicleId <= 0)
			{
				return BadRequest(new
				{
					error = "InvalidParameter",
					message = "VehicleId must be greater than zero."
				});
			}

			if (dto.StartDate >= dto.EndDate)
			{
				return BadRequest(new
				{
					error = "InvalidParameter",
					message = "End date must be greater than start date."
				});
			}

			var reservation = new G2Reservation
			{
				CustomerId = dto.CustomerId,
				VehicleId = dto.VehicleId,
				CreatedDate = DateTime.UtcNow,
				StartDate = dto.StartDate,
				EndDate = dto.EndDate
			};

			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();

			var result = new G2ReservationDto
			{
				Id = reservation.Id,
				CustomerId = reservation.CustomerId,
				VehicleId = reservation.VehicleId,
				CreatedDate = reservation.CreatedDate,
				StartDate = reservation.StartDate,
				EndDate = reservation.EndDate
			};

			return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, result);
		}
	}
}
