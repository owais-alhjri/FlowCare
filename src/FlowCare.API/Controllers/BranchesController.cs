using FlowCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/branches")]
    [ApiController]
    [AllowAnonymous]
    public class BranchesController(BranchesService branchesService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> FetchBranches()
        {
            var branches = await branchesService.FetchBranches();
            return Ok(branches);
        }
    }
}
