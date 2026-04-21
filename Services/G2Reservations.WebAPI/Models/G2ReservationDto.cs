using System.ComponentModel.DataAnnotations;

namespace G2Reservations.WebAPI.Models
{
	public class G2ReservationDto
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public int CustomerId { get; set; }
		[Required]
		public int VehicleId { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; }
		[Required]
		public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDate { get; set; }
	}
}
