using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swissbot_Worker.MessageTypes
{
    public class RolesUpdate
    {
        public string Type { get; set; } = "RolesUpdate";
        public string Action { get; set; }
        public ulong User { get; set; }
        public ulong[] Roles { get; set; }
    }
}
