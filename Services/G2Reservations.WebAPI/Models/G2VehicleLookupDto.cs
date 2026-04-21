using System.ComponentModel.DataAnnotations;

namespace G2Reservations.WebAPI.Models
{
	public class G2VehicleLookupDto
	{
		[Required]
		public int Id { get; set; }
		[Required]
		[StringLength(10, ErrorMessage = "Vehicle code cannot exceed 10 characters.")]
		public string VehicleCode { get; set; } = string.Empty;
		[Required]
		public int LocationId { get; set; }
		[Required]
		[StringLength(50, ErrorMessage = "Vehicle type cannot exceed 50 characters.")]
		public string VehicleType { get; set; } = string.Empty;
		[Required]
		[StringLength(50, ErrorMessage = "Vehicle status cannot exceed 50 characters.")]
		public string VehicleStatus { get; set; } = string.Empty;
	}
}
