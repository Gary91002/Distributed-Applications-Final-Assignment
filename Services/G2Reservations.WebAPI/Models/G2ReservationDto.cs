namespace G2Reservations.WebAPI.Models
{
	public class G2ReservationDto
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int VehicleId { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
