using AuthenticationService.Communication;

using NServiceBus;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AuthenticationService
{
    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    public partial class Server
    {
        /// <summary>
        /// Semaphore, used to indicate when a client connection has been recieved
        /// </summary>
        private ManualResetEvent connectionAttemptRecieved = new ManualResetEvent(false);

        private IEndpointInstance eventPublisher;
    }

    public partial class Server
    {

        public Server(IEndpointInstance eventPublisher)
        {
            this.eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Listen for incoming connections for as long as they continue to arrive until an exception is thrown
        /// </summary>
        public void StartListening()
        {
            // Establish the local endpoint for the socket.   
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IPv6 socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    connectionAttemptRecieved.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Messages.Debug.consoleMsg("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptConnection),
                        listener);

                    // Wait until a connection is made before continuing.  
                    connectionAttemptRecieved.WaitOne();
                }

            }
            catch (Exception e)
            {
                Messages.Debug.consoleMsg(e.ToString());
            }

        }

        /// <summary>
        /// Accepts the new connection and creates a new running thread to handle communication with the new connection
        /// </summary>
        /// <param name="ar"></param>
        public void AcceptConnection(IAsyncResult ar)
        {
            // Signal the main thread to continue listening for more connection attempts.  
            connectionAttemptRecieved.Set();

            // Get the socket that handles the client request.  
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket specificClientSocket = serverSocket.EndAccept(ar);
            
            ClientConnection connection = new ClientConnection(specificClientSocket, eventPublisher);

            Thread newThread = new Thread(new ThreadStart(connection.listenToClient));
            newThread.Start();
        }
        
    }
}
