using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CompanyReviewServiceEP.Communication
{
    public partial class CompanyReviewAPIRequest
    {

    }

    public partial class CompanyReviewAPIRequest
    {
        /// <summary>
        /// The port number the CompanyReviewService is listening from
        /// </summary>
        private const short port = 8080;

        /// <summary>
        /// The servername of the CompanyReviewService
        /// </summary>
        private const string serverName = "127.0.0.1";

        /// <summary>
        /// This object is used to make Web Api calls to the CompanyReviewService
        /// </summary>
        private static readonly HttpClient webCaller = initializeWebCaller();

        /// <summary>
        /// Initializes the webCaller
        /// </summary>
        /// <returns>WebCaller Object</returns>
        private static HttpClient initializeWebCaller()
        {
            HttpClient webCaller = new HttpClient();

            return webCaller;
        }
    }
}
