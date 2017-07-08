using NServiceBus;

using Messages.Commands;
using Messages.DataTypes;

namespace Messages.Events
{
    public class AccountCreated : IEvent
    {
        public AccountCreated(CreateAccount newAcct)
        {
            username = newAcct.username;
            password = newAcct.password;
            address = newAcct.address;
            phonenumber = newAcct.phonenumber;
            email = newAcct.email;
            type = newAcct.type;
        }

        public string username { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phonenumber { get; set; }
        public string email { get; set; }
        public AccountType type { get; set; }
    }
}
