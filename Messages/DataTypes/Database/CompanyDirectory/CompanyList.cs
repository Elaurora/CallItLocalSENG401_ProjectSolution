using NServiceBus;

namespace Messages.DataTypes.Database.CompanyDirectory
{
    /// <summary>
    /// Contains a list of company names
    /// </summary>
    public partial class CompanyList : IMessage
    {
        public CompanyList() { }

        /// <summary>
        /// This constructor parses given string representation of the object
        /// The string is assumed to be in the same format that is created by the
        /// user defined toString() function. Note the difference between the 
        /// ToString() method inherited from the object class
        /// </summary>
        /// <param name="info">The string representation created by toString()</param>
        public CompanyList(string info)
        {
            companyNames = info.Split(new char[] { '&' });
        }

        /// <summary>
        /// Creates a string representation of the current state of the object using a "rest-esque" type formatting
        /// </summary>
        /// <returns>The string representation of the object</returns>
        public string toString()
        {
            string returned = "";

            for(int i = 0; i != companyNames.Length; i++)
            {
                if(i != 0)
                {
                    returned += "&";
                }
                returned += companyNames[i];
            }

            return returned;
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    public partial class CompanyList : IMessage
    {
        /// <summary>
        /// A list of the company names
        /// </summary>
        public string[] companyNames { get; set; }
    }
}
