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
            if (userId == null)
            {
                return Unauthorized();
            }

            var logs = await auditLogService.ViewLogs(userId);
            return Ok(logs);
        }
    }
}
