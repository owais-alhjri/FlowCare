using System.Security.Claims;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to system audit logs, providing endpoints for viewing audit log entries.
    /// </summary>
    [Route("api/auditLog")]
    [ApiController]
    public class AuditLogController(AuditLogService auditLogService) : ControllerBase
    {

        /// <summary>
        /// Retrieves a list of system audit logs.
        /// </summary>
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