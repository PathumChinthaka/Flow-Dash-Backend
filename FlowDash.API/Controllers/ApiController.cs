using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowDash.API.Controllers
{
    [Authorize]
    [ApiController]
    public class ApiController : ControllerBase
    {
        protected IActionResult Created(object? data = null)
        {
            if (ReferenceEquals(data, null))
            {
                return StatusCode(201);
            }

            return StatusCode(201, data);
        }

        protected IActionResult Deleted(object? data = null)
        {
            if (ReferenceEquals(data, null))
            {
                return StatusCode(204);
            }

            return StatusCode(204, data);
        }
    }
}
