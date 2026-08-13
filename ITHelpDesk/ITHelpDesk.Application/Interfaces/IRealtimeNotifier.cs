using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITHelpDesk.Application.Interfaces
{
    public interface IRealtimeNotifier
    {
        Task SendToUserAsync(int userId, object payload);
    }
}
