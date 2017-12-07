using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Messages.ServiceBusRequest.CompanyReview.Requests
{
    [Serializable]
    [DataContract]
    public class GetCompanyReviewsRequest : CompanyReviewServiceRequest
    {
        public GetCompanyReviewsRequest(string companyName) 
            : base(CompanyReviewRequest.GetCompanyReviews)
        {
            this.companyName = companyName;
        }

        /// <summary>
        /// The name of the company to get reviews for
        /// </summary>
        [DataMember]
        private string companyName;
        public string getCompanyName() { return companyName; }
    }
}
