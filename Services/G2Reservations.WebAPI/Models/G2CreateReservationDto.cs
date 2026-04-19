using System.ComponentModel.DataAnnotations;

namespace G2Reservations.WebAPI.Models
{
	public class G2CreateReservationDto
	{
		[Required]
		public int CustomerId { get; set; }

		[Required]
		public int VehicleId { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }
	}
}
