using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G2Reservations.WebAPI.Models
{
	[Table("G2Reservation")]
	public class G2Reservation
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int CustomerId { get; set; }

		[Required]
		public int VehicleId { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime CreatedDate { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }
	}
}
