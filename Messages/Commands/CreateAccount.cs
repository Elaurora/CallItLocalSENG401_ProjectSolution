using NServiceBus;

using Messages.DataTypes;

namespace Messages.Commands
{
    public class CreateAccount : ICommand
    {
        public string username { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phonenumber { get; set; }
        public AccountType type { get; set; }
    }
}
