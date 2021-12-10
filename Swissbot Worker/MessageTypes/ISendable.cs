using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swissbot_Worker.MessageTypes
{
    interface ISendable
    {
        string type { get; }
        int workerId { get; set; }
        string session { get; set; }
    }
}
