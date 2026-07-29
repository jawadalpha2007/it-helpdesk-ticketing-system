using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Domain.Entities
{
    public class Status
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
