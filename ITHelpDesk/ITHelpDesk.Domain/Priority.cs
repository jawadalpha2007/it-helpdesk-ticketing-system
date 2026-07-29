using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Domain.Entities
{
    public class Priority
    {
        public int Id { get; set; }
        public string PriorityName { get; set; } = string.Empty;
    }
}
