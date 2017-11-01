using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.CompanyReview;
using Messages.ServiceBusRequest.CompanyReview.Responses;
using Messages.ServiceBusRequest.CompanyReview.Requests;


using NServiceBus;

using System;
using System.Collections.Generic;

namespace AuthenticationService.Communication
{
    /// <summary>
    /// This portion of the class contains methods used specifically for accessing the companyReviewService
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// Listens for the client to specify which task it is requesting from the CompanyReviewservice
        /// </summary>
        /// <param name="requestParameters">Informtation relevant to the task being requested</param>
        /// <returns>A response message</returns>
        private ServiceBusResponse companyReviewRequest(CompanyReviewServiceRequest request)
        {
            switch (request.requestType)
            {
                case (CompanyReviewRequest.GetCompanyReviews):
                    return getCompanyReviews((GetCompanyReviewsRequest)request);
                case (CompanyReviewRequest.SaveCompanyReview):
                    return saveNewReview((SaveCompanyReviewRequest)request);
                default:
                    return new ServiceBusResponse(false, "Error: Invalid request. Request received was:" + request.requestType.ToString());
            }
        }

        /// <summary>
        /// Makes a request to the Company Review Service for the customer reviews of a specific company
        /// </summary>
        /// <param name="request">Request information</param>
        /// <returns>The response from the service, containing the reviews</returns>
        private GetCompanyReviewsResponse getCompanyReviews(GetCompanyReviewsRequest request)
        {
            if (authenticated == false)
            {
                return new GetCompanyReviewsResponse(false, "Must be logged in to use the Company Directory Service", null);
            }

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CompanyReview");

            return requestingEndpoint.Request<GetCompanyReviewsResponse>(request, sendOptions)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Makes a request to the Company Review Service to save a new Customer Review
        /// </summary>
        /// <param name="request">Request Information</param>
        /// <returns>The response from the service</returns>
        private ServiceBusResponse saveNewReview(SaveCompanyReviewRequest request)
        {
            if (authenticated == false)
            {
                return new ServiceBusResponse(false, "Must be logged in to use the Company Directory Service");
            }

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CompanyDirectory");

            requestingEndpoint.Request<GetCompanyReviewsResponse>(request, sendOptions);

            return new ServiceBusResponse(true, "Success");
        }
    }
}
