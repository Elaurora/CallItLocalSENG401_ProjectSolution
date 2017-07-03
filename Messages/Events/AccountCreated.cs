using NServiceBus;

using Messages.DataTypes;

namespace Messages.Events
{
    public class AccountCreated : IEvent
    {
        public string username { get; set; }
        public string password { get; set; }
        public string phonenumber { get; set; }
        public AccountType type { get; set; }
    }
}
