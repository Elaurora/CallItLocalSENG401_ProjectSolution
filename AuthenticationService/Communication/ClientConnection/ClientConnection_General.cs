using Messages;
using Messages.DataTypes;

using NServiceBus;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AuthenticationService.Communication
{

    /// <summary>
    /// This portion of the class contains the function definitions
    /// 
    /// This class is used to communicate with clients that wish to use the services the bus has
    /// The client will continue to attempt to read a username and password until a valid pair has
    /// been given
    /// </summary>
    partial class ClientConnection
    {
        public ClientConnection(Socket connection, IEndpointInstance eventPublishingEndpoint)
        {
            this.connection = connection;
            this.eventPublishingEndpoint = eventPublishingEndpoint;
        }

        /// <summary>
        /// listens for message requests/commands for as long as the connection remains open.
        /// </summary>
        public void listenToClient()
        {
            while (connection.Connected == true)
            {
                //Read the name of the service, the taskrequested, and information associated with it.
                string serviceRequested = readUntilEOF();

                List<string> requestParameters = new List<string>(serviceRequested.Split('/'));

                string responseMessage = executeRequest(requestParameters);

                sendToClient(responseMessage);

                if (authenticated == false)
                {
                    terminateConnection();
                }
            }

            Debug.consoleMsg("Client connection closing...");

            return;
        }

        /// <summary>
        /// Executes the request received from the server
        /// </summary>
        /// <param name="requestParameters">Information about the request</param>
        /// <returns>A string representing the result of the request</returns>
        private string executeRequest(List<string> requestParameters)
        {
            if (requestParameters.Count < 2)
            {
                return "Error: Invalid Request. Are you using the correct syntax ?";
            }

            string serviceRequested = requestParameters[0];
            requestParameters.RemoveAt(0);

            switch (serviceRequested)
            {
                case ("authentication"):
                    return authenticationRequest(requestParameters);
                case ("companydirectory"):
                    return companyDirectoryRequest(requestParameters);
                case ("chat"):
                    return chatRequest(requestParameters);
                default:
                    return ("Error: Invalid request. Did not specify a valid service type. Specified type was: " + serviceRequested);
            }
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the "<EOF>" string of characters is found
        /// </summary>
        /// <returns>The string representation of bytes read from the client socket</returns>
        private string readUntilEOF()
        {
            byte[] readByte = new byte[1];
            string returned = String.Empty;

            while (returned.Contains(SharedData.msgEndDelim) == false)
            {
                try
                {
                    connection.Receive(readByte, 1, 0);
                    returned += (char)readByte[0];
                }
                catch (SocketException)// This is thrown when the timeout occurs. The timeout is set in the constructor
                {
                    Thread.Yield();// Yield this threads remaining timeslice to another process, this process does not appear to need it
                }
            }

            return returned.Substring(0, returned.IndexOf(SharedData.msgEndDelim));
        }

        /// <summary>
        /// Sends the given message to the client along with the msgEndDelim on the end
        /// </summary>
        /// <param name="msg">The message to send to the client</param>
        private void sendToClient(string msg)
        {
            if (connection.Connected == true && !"".Equals(msg))
            {
                connection.Send(Encoding.ASCII.GetBytes(msg + SharedData.msgEndDelim));
            }
        }

        /// <summary>
        /// Closes the connection with the client
        /// </summary>
        private void terminateConnection()
        {
            closeRequestEndpoint();
            connection.Disconnect(false);
        }

        /// <summary>
        /// Closes the requesting endpoint and sets its reference to null
        /// </summary>
        private void closeRequestEndpoint()
        {
            requestingEndpoint.Stop().ConfigureAwait(false).GetAwaiter().GetResult();
            requestingEndpoint = null;
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// The endpoint, used to communicate with other services in the bus.
        /// All clients use the same endpoint
        /// </summary>
        private IEndpointInstance requestingEndpoint = null;

        /// <summary>
        /// This endpoint is used to raise events.
        /// This second endpoint is used by all connections, and exists because other services need to attach
        /// to a single endpoint instance upon creation, and cannot continuously reattach to every clientconnection
        /// endpoint as they are created.
        /// </summary>
        private IEndpointInstance eventPublishingEndpoint;

        /// <summary>
        /// The connection between the client and the server.
        /// </summary>
        private Socket connection;

        /// <summary>
        /// This is the timeout used by the socket as it waits for a message from the client
        /// </summary>
        private const int timeout_ms = 50;
    }

    partial class ClientConnection
    {
        /// <summary>
        /// Returns an endpoint configuration object to be used to craete a new endpoint instance
        /// </summary>
        /// <param name="addressableName">The uniquely addressable ID of the endpoint</param>
        /// <returns>Endpoint Configuration Object with relevant settings for use with this server</returns>
        private static EndpointConfiguration getConfig(string addressableName)
        {
            //Create a new Endpoint configuration with the name "Authentication"
            EndpointConfiguration endpointConfiguration = new EndpointConfiguration("Authentication");

            //Allows the endpoint to run installers upon startup. This includes things such as the creation of message queues.
            endpointConfiguration.EnableInstallers();
            //Instructs the queue to serialize messages with Json, should it need to serialize them
            endpointConfiguration.UseSerialization<JsonSerializer>();
            //Instructs the endpoint to use local RAM to store queues. TODO: Good during development, not during deployment (According to the NServiceBus tutorial)
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            //Instructs the endpoint to send messages it cannot process to a queue named "error"
            endpointConfiguration.SendFailedMessagesTo("error");
            //Allows the endpoint to make requests to other endpoints and await a response.
            endpointConfiguration.EnableCallbacks();

            //Instructs the endpoint to use Microsoft Message Queuing TOD): Consider using RabbitMQ instead, only because Arcurve reccomended it. 
            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
            //This variable is used to configure how messages are routed. Using this, you may set the default reciever of a particular command, and/or subscribe to any number of events
            var routing = transport.Routing();

            endpointConfiguration.MakeInstanceUniquelyAddressable(addressableName);
            
            return endpointConfiguration;
        }
    }
}
