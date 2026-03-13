using FlowCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/branch")]
    [ApiController]
    [AllowAnonymous]
    public class BranchesController(BranchesService branchesService) : ControllerBase
    {
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