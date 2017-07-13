using AuthenticationService.Database;

using Messages.DataTypes;
using Messages.Commands;
using Messages.Events;
using Messages.Message;

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
        /// listens for message requests/commands until the socket is closed.
        /// </summary>
        public void listenToClient()
        {
            while (connection.Connected == true)
            {
                //TODO: Implement logic to deal with other messages that the web client may send

                //Read the name of the service the client wishes to access
                string serviceRequested = authenticator.readUntilEOF();

                switch (serviceRequested)
                {
                    case ("authentication"):
                        authenticationRequest();
                        break;
                    case ("companydirectory"):
                        companyDirectoryRequest();
                        break;
                    default:
                        sendToClient("Error: Invalid request. Request received was:" + serviceRequested);
                        break;
                }
            }
            return;
        }

        /// <summary>
        /// Listens for the client to specify which task it is requesting from the CompanyDirectoryservice
        /// </summary>
        private void companyDirectoryRequest()
        {
            string taskRequested = authenticator.readUntilEOF();

            switch (taskRequested)
            {
                case ("companysearch"):
                    searchForCompany();
                    break;
                default:
                    sendToClient("Error: Invalid request. Request received was:" + taskRequested);
                    break;
            }
        }

        private void searchForCompany()
        {
            //TODO: Make sure user is authenticated
            string companyToSearchFor = authenticator.readUntilEOF();

            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CompanyDirectory");

            CompanyList response = authenticationEndpoint.Request<CompanyList>(new SearchForCompany
                    {
                        delim = companyToSearchFor
                    }
                , sendOptions).ConfigureAwait(false).GetAwaiter().GetResult();

            sendToClient(response.toString());
        }

        /// <summary>
        /// Listens for the client to specify which task it is requesting from the authentication service
        /// </summary>
        private void authenticationRequest()
        {
            string taskRequested = authenticator.readUntilEOF();

            switch (taskRequested)
            {
                case ("login"):
                    authenticateUser();
                    break;
                case ("createaccount"):
                    readNewAccountInfo();
                    break;
                default:
                    sendToClient("Error: Invalid request. Request received was:" + taskRequested);
                    break;
            }
        }

        /// <summary>
        /// Reads the new account info from the client, parses it, and sends a "CreateAccount" command to the authentication endpoint
        /// It also sends a reply back to the client, and publishes an "AccountCreated" event
        /// </summary>
        private void readNewAccountInfo()
        {
            String info = authenticator.readUntilEOF();

            CreateAccount command = new CreateAccount(info);

            string[] separatedInfo = info.Split(new char[]{ '&' });

            if(AuthenticationDatabase.getInstance().insertNewUserAccount(command) == true)
            {
                authenticator.setAuthenticated(true);
                sendToClient("Success");
                authenticationEndpoint.Publish(new AccountCreated(command));
                return;
            }

            sendToClient("Failure");
            terminateConnection();
        }

        /// <summary>
        /// Wait for username and password to be sent, check the validity of the information sent,
        /// rinse repeat until authenticated. Sends a message to the client indicating whether or not 
        /// it was authenticated successfully
        /// </summary>
        private void authenticateUser()
        {
            authenticator.readLoginInfo();
            authenticator.attemptToAuthenticate();
            if (authenticator.isAuthenticated() == true)
            {
                sendToClient("Success");
                return;
            }
            sendToClient("Failure");
            terminateConnection();
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
        }

        /// <summary>
        /// Closes the connection with the client
        /// </summary>
        private void terminateConnection()
        {
            connection.Disconnect(false);
        }
    }
}
