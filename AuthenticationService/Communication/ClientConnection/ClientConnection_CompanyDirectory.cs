

using Messages.DataTypes.Database.CompanyDirectory;
using Messages.Commands;

using NServiceBus;

using System.Collections.Generic;

namespace AuthenticationService.Communication
{
    partial class ClientConnection
    {
        /// <summary>
        /// Listens for the client to specify which task it is requesting from the CompanyDirectoryservice
        /// </summary>
        private string companyDirectoryRequest(List<string> requestParameters)
        {
            if (authenticated == false)
            {
                return ("Error: You must be logged in to use the CompanyDirectoryService.");
            }

            string taskRequested = requestParameters[0];
            requestParameters.RemoveAt(0);

            switch (taskRequested)
            {
                case ("companysearch"):
                    return searchForCompany(requestParameters[0]);
                case ("getcompany"):
                    return getCompanyByName(requestParameters[0]);
                default:
                    return ("Error: Invalid request. Request received was:" + taskRequested);
            }
        }

        /// <summary>
        /// Asks the CompanyDirectoryService to search for a company matching the given description.
        /// </summary>
        /// <param name="companyToSearchFor">The name of the company to search for</param>
        /// <returns>A string representing the result of the request</returns>
        private string searchForCompany(string companyToSearchFor)
        {

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CompanyDirectory");

            CompanyList response = authenticationEndpoint.Request<CompanyList>(new SearchForCompany
            {
                delim = companyToSearchFor
            }
                , sendOptions).ConfigureAwait(false).GetAwaiter().GetResult();

            return (response.toString());
        }

        /// <summary>
        /// Gets the database to return information about the given company
        /// </summary>
        /// <param name="companyName">The name of the company to search for</param>
        /// <returns>String representation of the company being searched for</returns>
        private string getCompanyByName(string companyName)
        {
            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CompanyDirectory");

            CompanyInstance returned = authenticationEndpoint.Request<CompanyInstance>(new GetCompanyInfo
            {
                companyName = companyName
            }, sendOptions).ConfigureAwait(false).GetAwaiter().GetResult();

            if (returned.companyName == null)
            {
                return ("Error: Could not find company " + companyName);
            }

            return (returned.toString());
        }
    }
}
