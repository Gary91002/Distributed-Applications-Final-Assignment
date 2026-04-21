using System.ComponentModel.DataAnnotations;

namespace G2Maintenance.Application.DTOs;

public class G2AddRepairHistoryDTO
{
	[Required]
	public int VehicleId { get; set; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTime RepairDate { get; set; }
	[Required]
	[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
	public string Description { get; set; } = string.Empty;
	[Required]
	[Range(0, double.MaxValue, ErrorMessage = "Cost must be a non-negative value.")]
	public decimal Cost { get; set; }
	[Required]
	[StringLength(100, ErrorMessage = "PerformedBy cannot exceed 100 characters.")]
	public string PerformedBy { get; set; } = string.Empty;
}