using System.ComponentModel.DataAnnotations;

namespace G2Reservations.WebAPI.Models
{
	public class G2CustomerLookupDto
	{
		[Required]
		public int Id { get; set; }
		[Required]
		[StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
		public string FirstName { get; set; } = string.Empty;
		[Required]
		[StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
		public string LastName { get; set; } = string.Empty;
		[Required]
		[StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
		public string Phone { get; set; } = string.Empty;
		[Required]
		[StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
		public string Email { get; set; } = string.Empty;
	}
}
