using NServiceBus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Commands
{
    public class ReverseEcho : IMessage
    {
        public string data { get; set; }
        public string username { get; set; } = "";
    }
}
