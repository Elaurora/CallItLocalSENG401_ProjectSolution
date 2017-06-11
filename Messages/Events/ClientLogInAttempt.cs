using NServiceBus;

using System.Net;

namespace Messages.Events
{
    public class ClientLogInAttempt : IMessage { 
        public string username { get; set; }
        public string password { get; set; }
        public bool clientAccepted { get; set; }
        public SocketAddress requestSource { get; set; }
    }
}
