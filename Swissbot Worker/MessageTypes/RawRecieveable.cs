using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swissbot_Worker.MessageTypes
{
    public class RawRecieveable : IRecieveable
    {
        public string Type { get; set; }
    }
}
