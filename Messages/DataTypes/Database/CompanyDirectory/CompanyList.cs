using NServiceBus;

namespace Messages.DataTypes.Database.CompanyDirectory
{
    public class CompanyList : IMessage
    {
        /// <summary>
        /// A list of the company names
        /// </summary>
        public string[] companyNames { get; set; }


        public CompanyList() { }
        public CompanyList(string info)
        {
            companyNames = info.Split(new char[] { '&' });
        }

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
}
