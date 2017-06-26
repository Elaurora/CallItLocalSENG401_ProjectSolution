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
        /// <summary>
        /// The endpoint used to communicate with the other endpoints in the service bus
        /// </summary>
        private IEndpointInstance endpoint;

        /// <summary>
        /// A list of all the currently running client connections
        /// </summary>
        private List<Thread> threads = new List<Thread>();

    }

    public partial class Server
    {
        public Server(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
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
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptConnection),
                        listener);

                    // Wait until a connection is made before continuing.  
                    connectionAttemptRecieved.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            
            ClientConnection connection = new ClientConnection(handler, endpoint);

            Thread newThread = new Thread(new ThreadStart(connection.listenToClient));
            threads.Add(newThread);
            newThread.Start();
        }
        
    }
}
