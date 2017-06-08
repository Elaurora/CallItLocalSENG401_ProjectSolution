using NServiceBus;

namespace Messages
{
    public class AuthenticationResult : IMessage
    {
        public bool success { get; set; }
    }
}
