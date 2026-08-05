using ITHelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("agents")]
        public async Task<IActionResult> GetAgents([FromServices] AppDbContext context)
        {
            var agents = await context.Users
                .Where(u => u.RoleId == 2 && u.IsActive) // 2 = IT Support Agent
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            return Ok(agents);
        }

    }
}
