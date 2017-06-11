using NServiceBus;

using System.Net;

namespace Messages.Events
{
    /// <summary>
    /// This event contains information about a client that has attempted to connect to the bus
    /// It contains the username and password used, whether or not the pair was valid,
    /// and the Address the request came from.
    /// This event is published by the authentication service every time a client attempts to log in
    /// </summary>
    public class ClientLogInAttempted : IEvent { 
        public string username { get; set; }
        public string password { get; set; }
        public bool clientAccepted { get; set; }
        public SocketAddress requestSource { get; set; }
    }
}
