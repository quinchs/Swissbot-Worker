using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swissbot_Worker.MessageTypes
{
    public class MuteUsers : IRecieveable
    {
        public string Type { get; set; } = "MuteUsers";
        public string Action { get; set; }
        public bool Value { get; set; }
        public ulong[] Users { get; set; }
    }
}
