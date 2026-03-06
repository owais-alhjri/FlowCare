using FlowCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowCare.API.Controllers
{
    [Route("api/branches/{branchId}/services")]
    [ApiController]
    [AllowAnonymous]
    public class ServicesController(ServiceTypeService serviceTypeService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> FetchServiceByBranch(string branchId)
        {
            var services = await serviceTypeService.FetchServiceByBranch(branchId);
            return Ok(services);
        }

    }
}
