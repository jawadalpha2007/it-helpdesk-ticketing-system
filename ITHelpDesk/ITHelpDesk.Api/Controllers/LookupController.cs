using ITHelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ITHelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public  class LookupController : ControllerBase 
    {
      
            private readonly ILookupService _lookupService;

            public LookupController(ILookupService lookupService)
            {
                _lookupService = lookupService;
            }

            [HttpGet("categories")]
            public async Task<IActionResult> GetCategories()
            {
                var categories = await _lookupService.GetCategoriesAsync();
                return Ok(categories);
            }

            [HttpGet("priorities")]
            public async Task<IActionResult> GetPriorities()
            {
                var priorities = await _lookupService.GetPrioritiesAsync();
                return Ok(priorities);
            }

            [HttpGet("statuses")]
            public async Task<IActionResult> GetStatuses()
            {
                var statuses = await _lookupService.GetStatusesAsync();
                return Ok(statuses);
            }
        

        }
}
