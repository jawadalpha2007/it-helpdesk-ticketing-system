using ITHelpDesk.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Application.Interfaces
{
    public interface ILookupService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<List<PriorityDto>> GetPrioritiesAsync();
        Task<List<StatusDto>> GetStatusesAsync();
    }
}
