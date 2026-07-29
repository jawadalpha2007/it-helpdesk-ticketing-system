using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Services
{
    public class LookupService : ILookupService
    {
        private readonly AppDbContext _context;

        public LookupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto { Id = c.Id, CategoryName = c.CategoryName })
                .ToListAsync();
        }

        public async Task<List<PriorityDto>> GetPrioritiesAsync()
        {
            return await _context.Priorities
                .Select(p => new PriorityDto { Id = p.Id, PriorityName = p.PriorityName })
                .ToListAsync();
        }

        public async Task<List<StatusDto>> GetStatusesAsync()
        {
            return await _context.Statuses
                .Select(s => new StatusDto { Id = s.Id, StatusName = s.StatusName })
                .ToListAsync();
        }

    }
}
