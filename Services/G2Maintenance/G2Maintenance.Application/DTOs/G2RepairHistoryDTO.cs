using System.ComponentModel.DataAnnotations;

namespace G2Maintenance.Application.DTOs;

public class G2RepairHistoryDTO
{
	[Required]
	public int Id { get; set; }
	[Required]
	public int VehicleId { get; set; }
	public DateTime RepairDate { get; set; }
	public string Description { get; set; } = string.Empty;
	public decimal Cost { get; set; }
	public string PerformedBy { get; set; } = string.Empty;
}