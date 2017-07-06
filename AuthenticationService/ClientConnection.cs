using Messages.DataTypes;
using Messages.Commands;

using NServiceBus;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AuthenticationService
{
    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// The endpoint, used to communicate with other services in the bus.
        /// All clients use the same endpoint
        /// </summary>
        private IEndpointInstance authenticationEndpoint;

        /// <summary>
        /// The connection between the client and the server.
        /// </summary>
        private Socket connection;

        /// <summary>
        /// This class will be used to authenticate the client
        /// </summary>
        private ClientAuthenticator authenticator;
    }

    /// <summary>
    /// This portion of the class contains the function definitions
    /// 
    /// This class is used to communicate with clients that wish to use the services the bus has
    /// The client will continue to attempt to read a username and password until a valid pair has
    /// been given
    /// </summary>
    partial class ClientConnection
    {
        public ClientConnection(Socket connection, IEndpointInstance authenticationEndpoint)
        {
            this.connection = connection;
            this.authenticationEndpoint = authenticationEndpoint;
            authenticator = new ClientAuthenticator(connection, authenticationEndpoint);
        }

        /// <summary>
        /// Waits for the client to be authenticated, then listens for message requests/commands until the socket is closed.
        /// </summary>
        public async void listenToClient()
        {
            //waitUntilAuthenticated();
            while (connection.Connected == true)
            {
                //TODO: Implement logic to deal with other messages that the web client may send

                //Read the name of the service the client wishes to access
                string serviceRequested = authenticator.readUntilEOF();

                switch (serviceRequested)
                {
                    case ("login"):
                        authenticator.readLoginInfo();
                        break;
                    case ("createaccount"):
                        readNewAccountInfo();
                        break;
                    default:
                        sendToClient("Error: Invalid request. Request received was:" + serviceRequested);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the new account info from the client, parses it, and sends a "CreateAccount" command to the authentication endpoint
        /// It also sends a reply back to the client, and publishes an "AccountCreated" event
        /// </summary>
        private void readNewAccountInfo()
        {
            String info = authenticator.readUntilEOF();

            CreateAccount command = new CreateAccount
            {
                username = "",
                password = "",
                address = "",
                phonenumber = "",
                type = AccountType.NotSpecified
            };

            string[] separatedInfo = info.Split(new char[]{ '&' });

            try
            {
                foreach (string Info in separatedInfo)
                {
                    string[] pieces = Info.Split(new char[] { '=' });
                    switch (pieces[0])
                    {
                        case ("username"):
                            command.username = pieces[1];
                            break;
                        case ("password"):
                            command.password = pieces[1];
                            break;
                        case ("address"):
                            command.address = pieces[1];
                            break;
                        case ("phonenumber"):
                            command.phonenumber = pieces[1];
                            break;
                        case ("type"):
                            command.type = pieces[1] == "User" ? AccountType.User : AccountType.Business;
                            break;
                        default:
                            throw new ArgumentException("Error: Invalid or unknown argument given, aborting");
                    }
                }
            }
            catch(ArgumentException e)
            {
                sendToClient(e.Message);
                //TODO: Should i close the connection here ?
                return;
            }

            authenticationEndpoint.SendLocal(command);

            sendToClient("Success");
        }

        /// <summary>
        /// Wait for username and password to be sent, check the validity of the information sent,
        /// rinse repeat until authenticated. Sends a message to the client indicating whether or not 
        /// it was authenticated successfully
        /// </summary>
        private void waitUntilAuthenticated()
        {
            while (true)
            {
                authenticator.readLoginInfo();
                if(authenticator.isAuthenticated() == true)
                {
                    connection.Send(Encoding.ASCII.GetBytes("You have been logged in successfully<EOF>"));
                    break;
                }
                connection.Send(Encoding.ASCII.GetBytes("Incorrect username or password<EOF>"));
            }
        }

        /// <summary>
        /// Sends the given message to the client along with the msgEndDelim on the end
        /// </summary>
        /// <param name="msg">The message to send to the client</param>
        private void sendToClient(string msg)
        {
            if(connection.Connected == true)
            {
                connection.Send(Encoding.ASCII.GetBytes(msg + SharedData.msgEndDelim));
            }
            else
            {
                //TODO: Figure out what to do here
            }
        }
    }
}
