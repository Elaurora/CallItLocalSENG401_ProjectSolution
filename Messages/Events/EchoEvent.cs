using NServiceBus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Events
{
    public class EchoEvent : IEvent
    {
        public string data { get; set; }
        public string username { get; set; } = "";
    }
}
