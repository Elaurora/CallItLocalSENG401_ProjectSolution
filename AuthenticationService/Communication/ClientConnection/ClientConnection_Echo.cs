using Messages.Commands;
using Messages.Events;

using NServiceBus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Communication
{
    /// <summary>
    /// This portion of the class contains methods specifically for accessing the echo service.
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// Listens for the client to secifify which task is being requested from the echo service
        /// </summary>
        /// <param name="requestParameters">Includes which task is being requested and any additional information required for the task to be executed</param>
        /// <returns>A response message</returns>
        private string echoRequest(List<string> requestParameters)
        {
            string taskRequested = requestParameters[0];
            requestParameters.RemoveAt(0);

            switch (taskRequested)
            {
                case ("echo"):
                    return echoForward(requestParameters[0]);
                case ("reverse"):
                    return echoReverse(requestParameters[0]);
                default:
                    return ("Error: Invalid Request. Request received was:" + taskRequested);
            }
        }

        /// <summary>
        /// Publishes an EchoEvent.
        /// </summary>
        /// <param name="data">The data to be echo'd back to the client</param>
        /// <returns>The data sent by the client</returns>
        private string echoForward(string data)
        {
            EchoEvent echo = new EchoEvent
            {
                data = data
            };
            eventPublishingEndpoint.Publish(echo);
            return data;
        }

        /// <summary>
        /// Sends the data to the echo service, and returns the response.
        /// </summary>
        /// <param name="data">The data sent by the client</param>
        /// <returns>The response from the echo service</returns>
        private string echoReverse(string data)
        {
            if(authenticated == false)
            {
                return ("Error: You must be logged in to use the echo reverse functionality.");
            }

            // This class indicates to the request function where 
            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("Echo");

            // This is the object passed between this process and the echo service process
            ReverseEcho reverseEchoRequest = new ReverseEcho();
            reverseEchoRequest.data = data;

            // The Request funtion is an asynchronous function. However, since we do not want to continue execution until the Reuest
            // function runs to completion, we call the ConfigureAwait, GetAwaiter, and GetResult functions to ensure that this thread
            // will wait for the completion of Request before continueing. 
            reverseEchoRequest = requestingEndpoint.Request<ReverseEcho>(reverseEchoRequest, sendOptions).
                ConfigureAwait(false).GetAwaiter().GetResult();

            //return the response to the client.
            return reverseEchoRequest.data;
        }
    }
}
