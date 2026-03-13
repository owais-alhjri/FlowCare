using System.Security.Claims;
using FlowCare.Application.Interfaces;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController(
        SlotService slotService,
        ICsvExportService csvExportService,
        AuditLogService auditLogService) : ControllerBase
    {
        [HttpDelete("slots/cleanup")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> CleanUpSlots()
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminId is null)
                return Unauthorized();

            var result = await slotService.CleanUpSlots(adminId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new { Message = $"Successfully deleted {result.Value} slot(s)" });
        }

        [HttpGet("export/auditLog")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> ExportAuditLogToCsv()
        {
            var result = await auditLogService.AllLogs();
            if (result.IsFailure)
                return BadRequest(result.Error);

            var csvBytes = csvExportService.Export(result.Value!);
            return File(csvBytes, "text/csv", "audit-logs.csv");
        }
    }
}