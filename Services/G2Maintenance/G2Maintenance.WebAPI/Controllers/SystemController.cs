using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace G2Maintenance.WebAPI.Controllers
{
	[Route("")]
	[ApiController]
	public class SystemController : ControllerBase
	{
		[HttpGet("health")]
		public IActionResult Health()
		{
			return Ok(new
			{
				status = "UP"
			});
		}
	}
}
