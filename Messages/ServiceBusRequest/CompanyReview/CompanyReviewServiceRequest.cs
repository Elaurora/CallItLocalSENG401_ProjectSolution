using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Messages.ServiceBusRequest.CompanyReview
{
    [Serializable]
    [DataContract]
    public class CompanyReviewServiceRequest : ServiceBusRequest
    {
        public CompanyReviewServiceRequest(CompanyReviewRequest requestType)
            : base(Service.CompanyReview)
        {
            this.requestType = requestType;
        }
        
        /// <summary>
        /// Indicates the type of request the client is seeking from the Company Directory Service
        /// </summary>
        public CompanyReviewRequest requestType;
    }

    public enum CompanyReviewRequest { GetCompanyReviews, SaveCompanyReview };
}
