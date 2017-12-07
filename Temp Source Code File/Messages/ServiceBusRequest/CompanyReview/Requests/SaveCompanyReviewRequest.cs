using Messages.DataTypes.Database.CompanyReview;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Messages.ServiceBusRequest.CompanyReview.Requests
{
    [Serializable]
    [DataContract]
    public class SaveCompanyReviewRequest : CompanyReviewServiceRequest
    {
        public SaveCompanyReviewRequest(CustomerReview review) 
            : base(CompanyReviewRequest.SaveCompanyReview)
        {
            this.review = review;
        }

        /// <summary>
        /// The review of the company to be saved
        /// </summary>
        [DataMember]
        private CustomerReview review;
        public CustomerReview getReview() { return review; }
    }
}
