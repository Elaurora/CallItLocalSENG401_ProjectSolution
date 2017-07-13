using Messages.Message;
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
            string taskRequested = requestParameters[0];
            requestParameters.RemoveAt(0);

            switch (taskRequested)
            {
                case ("companysearch"):
                    return searchForCompany(requestParameters[0]);
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
            if (authenticated == true || authenticationEndpoint == null)
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
            else
            {
                return ("You must be logged in to search for companies");
            }
        }

    }
}
