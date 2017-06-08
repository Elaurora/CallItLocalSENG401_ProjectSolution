using NServiceBus;

namespace Messages.Commands
{
    public class AuthenticateMe : ICommand
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
