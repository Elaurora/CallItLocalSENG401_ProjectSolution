using Messages;
using Messages.DataTypes;

using NServiceBus;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace AuthenticationService.Communication
{

    /// <summary>
    /// This portion of the class contains the nonstatic function definitions
    /// 
    /// This class is used to communicate with clients that wish to use the services the bus has
    /// The client will continue to attempt to read a username and password until a valid pair has
    /// been given
    /// 
    /// This particular file contains members and methods relevant to all services.
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// Constructor. Sets member variables and Sets up the secure connection stream from the socket
        /// </summary>
        /// <param name="connection">The socket which is connected to the client</param>
        /// <param name="eventPublishingEndpoint">This is needed because the other services which subscribe to authentication endpoint events need an existing endpoint to subscribe too and cannot subscribe to user specific endpoint instances</param>
        /// <param name="certificate">The certificate object used for secure communication</param>
        public ClientConnection(Socket connection, IEndpointInstance eventPublishingEndpoint, X509Certificate2 certificate)
        {
            this.connection = connection;
            this.eventPublishingEndpoint = eventPublishingEndpoint;
            this.certificate = certificate;
            this.connectionStream = new SslStream(new NetworkStream(connection, false));
            this.connectionStream.AuthenticateAsServer(certificate,
                   false, SslProtocols.Tls, true);
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
                case ("chat"):
                    return chatRequest(requestParameters);
                case ("companydirectory"):
                    return companyDirectoryRequest(requestParameters);
                default:
                    return ("Error: Invalid request. Did not specify a valid service type. Specified type was: " + serviceRequested);
            }
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the end of file string of characters is found
        /// The end of file string is found in the Messages library which is shared by the web server and the bus.
        /// </summary>
        /// <returns>The string representation of bytes read from the client socket</returns>
        private string readUntilEOF()
        {
            byte[] encodedBytes = new byte[2048];
            string returned = String.Empty;

            while (returned.Contains(SharedData.msgEndDelim) == false)
            {
                try
                {
                    //connection.Receive(encodedBytes, 1, 0);

                    int bytesRead = connectionStream.Read(encodedBytes, 0, encodedBytes.Length);

                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] decodedBytes = new char[decoder.GetCharCount(encodedBytes, 0, bytesRead)];

                    decoder.GetChars(encodedBytes, 0, bytesRead, decodedBytes, 0);
                    
                    returned += new string(decodedBytes);
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
                //connection.Send(Encoding.ASCII.GetBytes(msg + SharedData.msgEndDelim));

                msg += SharedData.msgEndDelim;
                connectionStream.Write(Encoding.UTF8.GetBytes(msg));
                connectionStream.Flush();
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
        /// This is the stream used to read and write to the socket securely
        /// </summary>
        private SslStream connectionStream = null;

        /// <summary>
        /// This is the timeout used by the socket as it waits for a message from the client
        /// </summary>
        private const int timeout_ms = 50;

        /// <summary>
        /// This is used to authenticate the connection between the client(web server) and the bus.
        /// </summary>
        private X509Certificate2 certificate = null;
    }

    /// <summary>
    /// This portion of the class contains the static members
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// Returns an endpoint configuration object to be used to craete a new endpoint instance
        /// This function is required because each endpoint configuration can only be associated with a single endpoint instance
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
