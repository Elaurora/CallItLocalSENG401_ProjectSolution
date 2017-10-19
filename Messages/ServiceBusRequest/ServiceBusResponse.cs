using NServiceBus;

using System;

namespace Messages.ServiceBusRequest
{
    [Serializable]
    public class ServiceBusResponse : IMessage
    {
        public ServiceBusResponse(bool result, string response)
        {
            this.result = result;
            this.response = response;
        }

        /// <summary>
        /// The result indicates whether or not the service bus was able to fulfill
        /// the client request. True for success, false for failure
        /// </summary>
        public bool result = false;

        /// <summary>
        /// The response indicates some information about the request, usually failure data
        /// </summary>
        public string response = "";
    }
}
