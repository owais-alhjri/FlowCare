using System.Security.Claims;
using FlowCare.Application.Interfaces;
using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for managing slots and exporting audit logs within the application.
    /// </summary>
    /// <remarks>Access to this controller is restricted to users authorized with the 'AdminOnly' policy. All
    /// endpoints are intended for administrative use only.</remarks>
    /// <param name="slotService">The service used to perform operations related to slot management, including cleanup of unused slots.</param>
    /// <param name="csvExportService">The service responsible for exporting data in CSV format.</param>
    /// <param name="auditLogService">The service that retrieves audit log entries for export and auditing purposes.</param>
    [Route("api/admin")]
    [ApiController]
    public class AdminController(
        SlotService slotService,
        ICsvExportService csvExportService,
        AuditLogService auditLogService) : ControllerBase
    {
        /// <summary>
        /// Deletes unused slots from the system and returns the result of the cleanup operation.
        /// </summary>
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
        /// <summary>
        /// Exports all audit log entries as a CSV file.
        /// </summary>
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