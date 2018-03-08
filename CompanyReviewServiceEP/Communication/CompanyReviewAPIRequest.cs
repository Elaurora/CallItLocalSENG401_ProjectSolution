using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.CompanyReview.Requests;
using Messages.ServiceBusRequest.CompanyReview.Responses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CompanyReviewServiceEP.Communication
{
    /// <summary>
    /// This class is used to make API calls over Http to the server running the Company Review Service
    /// </summary>
    public partial class CompanyReviewAPIRequest
    {
        /// <summary>
        /// Sends an API request over http to the server running the Company Review Service to save a new customer review
        /// </summary>
        /// <param name="request">Request Information</param>
        /// <returns>The response from the server</returns>
        public ServiceBusResponse saveCompanyReview(SaveCompanyReviewRequest request)
        {
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SaveCompanyReviewRequest));

            serializer.WriteObject(memStream, request);

            string jsonEndodedObject = Encoding.ASCII.GetString(memStream.ToArray());

            string path = $"SaveCompanyReview/{jsonEndodedObject}";
            //string path = $"GetCompanyReviews/{request.getCompanyName()}";
            HttpResponseMessage response;

            try
            {
                response = webCaller.GetAsync(path).GetAwaiter().GetResult();
            }
            catch (HttpRequestException e)
            {
                return new ServiceBusResponse(false, "Could not connect to service API. Error messsage: " + e.Message);
            }

            if(response.IsSuccessStatusCode == true)
            {
                return new ServiceBusResponse(true, "Success");
            }
            return new ServiceBusResponse(false, "Error code:" + response.StatusCode.ToString() + " Error Message:" + response.Content);
        }

        /// <summary>
        /// Sends an API request over http to the server running the Company Review Service requesting a list of Customer reviews for a specified company
        /// </summary>
        /// <param name="request">Request Information</param>
        /// <returns>The response from the server containing the requested reviews</returns>
        public GetCompanyReviewsResponse getCompanyReviews(GetCompanyReviewsRequest request)
        {
            HttpResponseMessage response;
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GetCompanyReviewsRequest));

            serializer.WriteObject(memStream, request);

            string jsonEndodedObject = Encoding.ASCII.GetString(memStream.ToArray());

            string path = $"GetCompanyReviews/{jsonEndodedObject}";
            //string path = $"GetCompanyReviews/{request.getCompanyName()}";

            try { 
                response = webCaller.GetAsync(path).GetAwaiter().GetResult();
            }
            catch (HttpRequestException e)
            {
                return new GetCompanyReviewsResponse(false, "Could not connect to service API. Error messsage: " + e.Message, null);
            }

            if (response.IsSuccessStatusCode == false)
            {
                return new GetCompanyReviewsResponse(false, response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), null);
            }
            
            memStream = (MemoryStream)response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();

            serializer = new DataContractJsonSerializer(typeof(GetCompanyReviewsResponse));
            GetCompanyReviewsResponse responseData = (GetCompanyReviewsResponse)serializer.ReadObject(memStream);

            /* I put this here so i could get the response as a JSON object more easily to use as an example.
            MemoryStream memStream1 = new MemoryStream();
            DataContractJsonSerializer serializer1 = new DataContractJsonSerializer(typeof(GetCompanyReviewsResponse));

            serializer1.WriteObject(memStream1, responseData);

            string jsonEndodedObject1 = Encoding.ASCII.GetString(memStream1.ToArray());
            */

            return responseData;
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
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
            webCaller.BaseAddress = new Uri($"http://{serverName}:{port}/CompanyReviewService_Java/");
            webCaller.DefaultRequestHeaders.Accept.Clear();
            webCaller.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            webCaller.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            return webCaller;
        }
    }
}
