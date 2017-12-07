using Messages.ServiceBusRequest.CompanyReview.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Messages.DataTypes.Database.CompanyReview
{
    /// <summary>
    /// This class contains all of the information needed to represent a customer's review of a company
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerReview
    {
        public CustomerReview(string companyName, string username, string review, int stars, int timestamp)
        {
            this.companyName = companyName;
            this.username = username;
            this.review = review;
            this.stars = stars;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// The name of the company the review is about
        /// </summary>
        [DataMember]
        private string companyName;
        public string getCompanyName() { return companyName; }

        /// <summary>
        /// The username of the user who wrote the review
        /// </summary>
        [DataMember]
        private string username;
        public string getUsername() { return username; }

        /// <summary>
        /// The review itself
        /// </summary>
        [DataMember]
        private string review;
        public string getReview() { return review; }

        /// <summary>
        /// The number of stars given to the company, between 0 and 5
        /// </summary>
        [DataMember]
        private int stars;
        public int getStars() { return stars; }

        /// <summary>
        /// A Unix timestamp indicating when the review was written
        /// </summary>
        [DataMember]
        private int timestamp;
        public int getTimestsamp() { return timestamp; }
    }
}
