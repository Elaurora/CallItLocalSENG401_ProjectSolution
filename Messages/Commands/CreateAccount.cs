using NServiceBus;

using Messages.DataTypes;

namespace Messages.Commands
{
    /// <summary>
    /// This class represents the command to create an account as well as all of the information needed to do so.
    /// </summary>
    public partial class CreateAccount : ICommand
    {
        public CreateAccount() { }

        /// <summary>
        /// This constructor parses given string representation of the object
        /// The string is assumed to be in the same format that is created by the
        /// user defined toString() function. Note the difference between the 
        /// ToString() method inherited from the object class
        /// </summary>
        /// <param name="info">The string representation</param>
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

        /// <summary>
        /// Creates a string representation of the current state of the object using a "rest-esque" type formatting
        /// </summary>
        /// <returns>The string representation of the object</returns>
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

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    public partial class CreateAccount : ICommand
    {
        /// <summary>
        /// The username of the new account
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// The password for the new account
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// The address of the new user
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// The phone number of the new user
        /// </summary>
        public string phonenumber { get; set; }

        /// <summary>
        /// the email of the new user
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// The type of account
        /// </summary>
        public AccountType type { get; set; }
    }
}
