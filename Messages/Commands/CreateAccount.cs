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
        public string email { get; set; }
        public AccountType type { get; set; }

        public CreateAccount() { }
        public CreateAccount(string info)
        {
            username = "";
            password = "";
            address = "";
            phonenumber = "";
            email = "";
            type = AccountType.notspecified;

            string[] separatedInfo = info.Split(new char[] { '&' });

            foreach (string Info in separatedInfo)
            {
                string[] pieces = Info.Split(new char[] { '=' });
                switch (pieces[0])
                {
                    case ("username"):
                        username = pieces[1];
                        break;
                    case ("password"):
                        password = pieces[1];
                        break;
                    case ("address"):
                        address = pieces[1];
                        break;
                    case ("phonenumber"):
                        phonenumber = pieces[1];
                        break;
                    case ("email"):
                        email = pieces[1];
                        break;
                    case ("type"):
                        type = pieces[1] == "user" ? AccountType.user : AccountType.business;
                        break;
                }
            }
        }

        public string toString()
        {
            return 
                "username=" + username +
                "&password=" + password +
                "&address=" + address +
                "&phonenumber=" + phonenumber +
                "&email=" + email +
                "&type=" + type.ToString();
        }
    }
}
