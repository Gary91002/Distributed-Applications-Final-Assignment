using System.ComponentModel.DataAnnotations;

namespace G2Reservations.WebAPI.Models
{
	public class G2ReservationHistoryDto
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public int CustomerId { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
		public string CustomerName { get; set; } = string.Empty;
		[Required]
		public int VehicleId { get; set; }

		[Required]
		[StringLength(10, ErrorMessage = "Vehicle code cannot exceed 10 characters.")]
		public string VehicleCode { get; set; } = string.Empty;
		[Required]
		[StringLength(50, ErrorMessage = "Vehicle type cannot exceed 50 characters.")]
		public string VehicleType { get; set; } = string.Empty;
		[Required]
		[StringLength(50, ErrorMessage = "Vehicle status cannot exceed 50 characters.")]
		public string VehicleStatus { get; set; } = string.Empty;
		[DataType(DataType.DateTime)]
		public DateTime CreatedDate { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }
	}
}
