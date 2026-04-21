using G2VehicleInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G2VehicleInventory.Application.DTOs
{
	public class G2CreateVehicleDto
	{
		[Required]
		[StringLength(10, MinimumLength = 1, ErrorMessage = "Vehicle code must be between 1 and 10 characters.")]
		public string VehicleCode { get; set; } = string.Empty;

		[Required]
		public int LocationId { get; set; }
		[Required]
		public VehicleType VehicleType { get; set; }
		[Required]
		public VehicleStatus VehicleStatus { get; set; }
	}
}
