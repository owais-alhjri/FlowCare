using FlowCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/branches")]
    [ApiController]
    [AllowAnonymous]
    public class BranchesController(BranchesService branchesService, ServiceTypeService serviceTypeService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> FetchBranches()
        {
            var branches = await branchesService.FetchBranches();
            return Ok(branches);
        }

        [HttpGet("{branchId}/services")]
        public async Task<ActionResult> FetchServiceByBranch( string branchId)
        {
            var servicesByBranch = await serviceTypeService.FetchServiceByBranch(branchId);

            return Ok(servicesByBranch);
        }

        
    }
}
