using NServiceBus;

using System;

namespace Messages.DataTypes.Database.CompanyDirectory
{
    public partial class CompanyInstance : IMessage
    {
        public CompanyInstance() { }

        /// <summary>
        /// Creates a company Instance object using the given string.
        /// The string is assumed to be in a format that was created by the toString
        /// method defined in the CompanyInstance definition
        /// </summary>
        /// <param name="stringForm">The string to parse</param>
        public CompanyInstance(String stringForm)
        {
            if (stringForm != null) {
                string[] parts = stringForm.Split('&');
                foreach (string part in parts)
                {
                    string[] pieces = part.Split('=');
                    switch (pieces[0])
                    {
                        case ("companyName"):
                            companyName = pieces[1];
                            break;
                        case ("phoneNumber"):
                            phoneNumber = pieces[1];
                            break;
                        case ("email"):
                            email = pieces[1];
                            break;
                        case ("locations"):
                            locations = pieces[1].Split('!');
                            break;
                        default:
                            throw new ArgumentException("Invalid format for CompanyInstance");
                    }
                } }
        }

        /// <summary>
        /// Creates a CompanyInstance object using the 
        /// </summary>
        /// <param name="companyName">The name of the company</param>
        /// <param name="phoneNumber">The phone number of the company</param>
        /// <param name="email">The email of the company</param>
        /// <param name="locations">An array of locations the company resides</param>
        public CompanyInstance(string companyName, string phoneNumber, string email, string[] locations)
        {
            this.companyName = companyName;
            this.phoneNumber = phoneNumber;
            this.email = email;
            this.locations = locations;
        }

        /// <summary>
        /// Creates a string representation of the CompanyInstance
        /// </summary>
        /// <returns>The string representation of this company instance</returns>
        public string toString()
        {
            string returned =
                "companyName=" + companyName + "&" +
                "phoneNumber=" + phoneNumber + "&" +
                "email=" + email + "&" +
                "locations=";
            for(int i = 0; i != locations.Length; i++)
            {
                if(i != 0)
                {
                    returned += "!";
                }
                returned += locations[i];
            }
            return returned;
        }
    }

    public partial class CompanyInstance : IMessage
    {
        public String companyName = null;

        public String phoneNumber = null;

        public String email = null;

        public String[] locations = null;
    }
}
