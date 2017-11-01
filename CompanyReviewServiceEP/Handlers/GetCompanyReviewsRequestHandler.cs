using CompanyReviewServiceEP.Communication;

using Messages.ServiceBusRequest.CompanyReview.Responses;
using Messages.ServiceBusRequest.CompanyReview.Requests;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace CompanyReviewServiceEP.Handlers
{
    /// <summary>
    /// This class is used by the Company Review Service endpoint when a client requests a list of reviews for a specific company
    /// </summary>
    public class GetCompanyReviewsRequestHandler : IHandleMessages<GetCompanyReviewsRequest>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        private static ILog log = LogManager.GetLogger<GetCompanyReviewsRequestHandler>();

        /// <summary>
        /// Makes a call to the CompanyReviewService's Web Api
        /// </summary>
        /// <param name="request">Object containing information about the request</param>
        /// <param name="context">Information regarding the calling endpoint</param>
        /// <returns></returns>
        public Task Handle(GetCompanyReviewsRequest request, IMessageHandlerContext context)
        {
            CompanyReviewAPIRequest webRequest = new CompanyReviewAPIRequest();
            GetCompanyReviewsResponse response = webRequest.getCompanyReviews(request);
            return context.Reply(response);
        }
    }
}
