using NServiceBus;

namespace Messages.Commands
{
    public enum AccountType { User, Business};

    public class CreateAccount : ICommand
    {
        public string username { get; set; }
        public string password { get; set; }
        public AccountType type { get; set; }
    }
}
