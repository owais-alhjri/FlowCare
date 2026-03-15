using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    /// <summary>
    /// Provides HTTP endpoints for retrieving branch information. This controller is intended for use in public APIs
    /// and allows anonymous access.
    /// </summary>
    [Route("api/branch")]
    [ApiController]
    [AllowAnonymous]
    public class BranchesController(BranchesService branchesService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a list of branches.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> FetchBranches()
        {
            var result = await branchesService.FetchBranches();
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}