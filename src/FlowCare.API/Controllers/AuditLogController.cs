using System.Security.Claims;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/auditLog")]
    [ApiController]
    public class AuditLogController(AuditLogService auditLogService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "ManagerOrAbove")]
        public async Task<ActionResult> ViewLogs()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await auditLogService.ViewLogs(userId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}