using Messages.DataTypes.Database.CompanyDirectory;
using Messages.NServiceBus.Commands;
using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.CompanyDirectory;
using Messages.ServiceBusRequest.CompanyDirectory.Responses;
using Messages.ServiceBusRequest.CompanyDirectory.Requests;

using NServiceBus;

using System.Collections.Generic;

namespace AuthenticationService.Communication
{
    /// <summary>
    /// This portion of the class contains methods used specifically for accessing the companyDirectoryService
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// Listens for the client to specify which task it is requesting from the CompanyDirectoryservice
        /// </summary>
        /// <param name="requestParameters">Informtation relevant to the task being requested</param>
        /// <returns>A response message</returns>
        private ServiceBusResponse companyDirectoryRequest(CompanyDirectoryServiceRequest request)
        {
            switch (request.requestType)
            {
                case (CompanyDirectoryRequest.CompanySearch):
                    return searchForCompany((CompanySearchRequest)request);
                case (CompanyDirectoryRequest.GetCompanyInfo):
                    return getCompanyInfo((GetCompanyInfoRequest)request);
                default:
                    return new ServiceBusResponse(false, "Error: Invalid request. Request received was:" + request.requestType.ToString());
            }
        }

        /// <summary>
        /// Asks the CompanyDirectoryService to search for a company matching the given description.
        /// </summary>
        /// <param name="companyToSearchFor">The name of the company to search for</param>
        /// <returns>A string representing the result of the request</returns>
        private CompanySearchResponse searchForCompany(CompanySearchRequest request)
        {
            if(authenticated == false)
            {
                return new CompanySearchResponse(false, "Must be logged in to use the Company Directory Service", null);
            }

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CompanyDirectory");

            return requestingEndpoint.Request<CompanySearchResponse>(request, sendOptions)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the database to return information about the given company
        /// </summary>
        /// <param name="companyName">The name of the company to search for</param>
        /// <returns>String representation of the company being searched for</returns>
        private GetCompanyInfoResponse getCompanyInfo(GetCompanyInfoRequest request)
        {
            if (authenticated == false)
            {
                return new GetCompanyInfoResponse(false, "Must be logged in to use the Company DIrectory Service", null);
            }

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CompanyDirectory");

            return requestingEndpoint.Request<GetCompanyInfoResponse>(request, sendOptions)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
