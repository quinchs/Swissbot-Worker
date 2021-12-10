using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swissbot_Worker.MessageTypes
{
    public class Log : ISendable
    {
        public string message { get; set; }
        public int workerId { get; set; }
        public string type { get; set; } = "Log";
        public string session { get; set; } = Program.SwissbotAuth;
    }
}
