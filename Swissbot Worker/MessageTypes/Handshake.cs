using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swissbot_Worker.MessageTypes
{
    class Handshake : ISendable
    {
        public string type { get; set; } = "handshake";
        public int workerId { get; set; } = Program.WorkerId;
        public string session { get; set; } = Program.SwissbotAuth;
    }
}
