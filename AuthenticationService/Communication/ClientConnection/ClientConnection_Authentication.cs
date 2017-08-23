using AuthenticationService.Database;

using Messages.Commands;
using Messages.Events;

using NServiceBus;

using System;
using System.Collections.Generic;
using System.Net;

namespace AuthenticationService.Communication
{
    /// <summary>
    /// This portion of the class contains methods specifially for accessing the authentication service
    /// </summary>
    partial class ClientConnection
    {
        /// <summary>
        /// Listens for the client to specify which task it is requesting from the authentication service
        /// </summary>
        /// <param name="requestParameters">Information regarding the task being requested</param>
        /// <returns>A response message</returns>
        private string authenticationRequest(List<string> requestParameters)
        {
            string taskRequested = requestParameters[0];
            requestParameters.RemoveAt(0);

            switch (taskRequested)
            {
                case ("login"):
                    return attemptToAuthenticateUser(requestParameters[0]);
                case ("createaccount"):
                    return attemptNewAccountCreation(requestParameters[0]);
                default:
                    return("Error: Invalid request. Request received was:" + taskRequested);
            }
        }

        /// <summary>
        /// Parses new account info and attempts to insert the new account into the authentication database
        /// It also publishes an "AccountCreated" event
        /// </summary>
        /// <returns>A response message</returns>
        private string attemptNewAccountCreation(string info)
        {
            CreateAccount command = new CreateAccount(info);

            string dbResponse = AuthenticationDatabase.getInstance().insertNewUserAccount(command);

            if ("Success".Equals(dbResponse))
            {
                authenticated = true;
                username = command.username;
                password = command.password;
                initializeRequestingEndpoint();
                eventPublishingEndpoint.Publish(new AccountCreated(command));
            }
            return dbResponse;
        }
        
        /// <summary>
        /// Parses username and password from info and checks the database for validity of the information sent.
        /// If invalid, closes the connection after sending the response. 
        /// If successful will keep the connection open
        /// </summary>
        /// <param name="info">Should contain the username and password in the proper format. See parseLoginInfo definition for desired format</param>
        /// <returns>A response message indicating the result of the attempt</returns>
        private string attemptToAuthenticateUser(string info)
        {
            parseLoginInfo(info);

            if ("".Equals(username) || "".Equals(password))
            {
                terminateConnection();
                return ("Failure. Username or password not sent properly");
            }

            authenticated = AuthenticationDatabase.getInstance().isValidUserInfo(username, password);
            reportLogInAttempt();

            if (authenticated == true)
            {
                return ("Success");
            }
            return ("Failure");
        }

        /// <summary>
        /// Publishes a ClientLogInAttempted event through the publishing endpoint
        /// </summary>
        private void reportLogInAttempt()
        {
            ClientLogInAttempted attempt = new ClientLogInAttempted
            {
                username = this.username,
                password = this.password,
                clientAccepted = this.authenticated,
                requestSource = ((IPEndPoint)connection.RemoteEndPoint).Serialize()
            };

            //Publish the log in attempt event for any other EP that wish to know about it.
            //If an endpoint wishes to be notified about this event, it should subscribe to the event in its configuration
            Console.Write("Log in attempted with credentials:" + "\n" +
                "Username:" + username + "\n" +
                "Password:" + password + "\n");

            if (authenticated == true)
            {
                initializeRequestingEndpoint();
            }
            eventPublishingEndpoint.Publish(attempt);
        }

        /// <summary>
        /// Attempts to convert the given string into a username and password.
        /// The expexted format is :
        /// "u=givenusername&p=givenpassword"
        /// </summary>
        private void parseLoginInfo(string info)
        {
            string[] components = info.Split('&');

            if (components.Length != 2)
            {
                username = "";
                password = "";
                return;
            }
            foreach (string component in components)
            {
                string[] pieces = component.Split('=');
                if (pieces.Length != 2)
                {
                    username = "";
                    password = "";
                    return;
                }
                switch (pieces[0])
                {
                    case ("u"):
                        username = pieces[1];
                        break;
                    case ("p"):
                        password = pieces[1];
                        break;
                    default:
                        username = "";
                        password = "";
                        return;
                }
            }
        }

        /// <summary>
        /// Starts the endoint that will be linked to this specific client connection
        /// </summary>
        private void initializeRequestingEndpoint()
        {
            EndpointConfiguration config = getConfig(username);
            requestingEndpoint = Endpoint.Start(config).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    partial class ClientConnection
    {

        /// <summary>
        /// The username given by the client
        /// </summary>
        private string username = "";

        /// <summary>
        /// the password given by the client
        /// </summary>
        private string password = "";

        /// <summary>
        /// Indicates whether or not the username and password given by the client are valid
        /// </summary>
        private bool authenticated = false;
    }
}
