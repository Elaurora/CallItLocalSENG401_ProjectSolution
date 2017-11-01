using Messages.DataTypes.Database.CompanyReview;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Messages.ServiceBusRequest.CompanyReview.Responses
{
    [Serializable]
    [DataContract]
    public class GetCompanyReviewsResponse : ServiceBusResponse
    {
        public GetCompanyReviewsResponse(bool result, string response, List<CustomerReview> reviews)
            : base(result, response)
        {
            this.reviews = reviews;
        }
        
        [DataMember]
        private List<CustomerReview> reviews;
        public List<CustomerReview> getReviews() { return reviews; }
    }
}
